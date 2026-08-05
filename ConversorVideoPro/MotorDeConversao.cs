using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ConversorVideoPro
{
    public class MotorDeConversao
    {
        private readonly string[] extensoesVideo = { ".mp4", ".mov", ".mkv", ".avi", ".flv", ".wmv", ".mpeg", ".mpg", ".webm", ".mxf", ".mts" };

        private Action<string, Color> _atualizarLog;
        private Action<string, string, int> _atualizarProgresso;

        public async Task IniciarProcessoAsync(string origem, string destino, string argumentosBase, bool apagarOriginais, Action<string, Color> atualizadorDeLog, Action<string, string, int> atualizadorProgresso, CancellationToken token)
        {
            _atualizarLog = atualizadorDeLog;
            _atualizarProgresso = atualizadorProgresso;

            var todosArquivos = Directory.GetFiles(origem, "*.*", SearchOption.AllDirectories);
            var arquivosVideo = todosArquivos.Where(f => extensoesVideo.Contains(Path.GetExtension(f).ToLower())).ToList();

            if (arquivosVideo.Count == 0)
            {
                _atualizarLog("Nenhum arquivo de vídeo suportado foi encontrado na origem.", Color.Yellow);
                return;
            }

            _atualizarLog($"Total de vídeos encontrados: {arquivosVideo.Count}\n", Color.Cyan);

            string melhorEncoder = ObterMelhorEncoderGpu();
            _atualizarLog($"Hardware selecionado para a TAG {{ENCODER}}: {melhorEncoder}\n", Color.Orange);

            string arquivoLogTxt = Path.Combine(destino, "log_conversoes.txt");
            Directory.CreateDirectory(destino);
            File.AppendAllText(arquivoLogTxt, $"==================================================\nLog de conversões - {DateTime.Now:dd/MM/yyyy HH:mm:ss}\nOpção de Exclusão de Originais: {apagarOriginais}\n==================================================\n");

            foreach (var arquivoOrigem in arquivosVideo)
            {
                if (token.IsCancellationRequested) break;

                string caminhoRelativo = arquivoOrigem.Substring(origem.Length).TrimStart('\\');
                string arquivoDestino = Path.Combine(destino, caminhoRelativo);
                arquivoDestino = Path.ChangeExtension(arquivoDestino, ".mp4");
                Directory.CreateDirectory(Path.GetDirectoryName(arquivoDestino));

                TimeSpan duracaoVideo = ObterDuracaoVideo(arquivoOrigem);
                _atualizarLog($"[Convertendo] {Path.GetFileName(arquivoOrigem)} (Duração: {duracaoVideo:hh\\:mm\\:ss})...", Color.White);

                string argumentosAtuais = argumentosBase.Replace("{ENCODER}", melhorEncoder);
                string argumentosFFmpeg = $"-y -i \"{arquivoOrigem}\" {argumentosAtuais} \"{arquivoDestino}\"";

                Stopwatch cronometro = Stopwatch.StartNew();
                long tamanhoOriginal = new FileInfo(arquivoOrigem).Length;

                bool sucesso = await ExecutarFFmpeg(argumentosFFmpeg, duracaoVideo, token);

                if (!sucesso && !token.IsCancellationRequested && melhorEncoder != "libx264" && argumentosBase.Contains("{ENCODER}"))
                {
                    _atualizarLog($"[FALHA NA GPU] A placa de vídeo recusou. Tentando fallback via Processador (CPU)...", Color.OrangeRed);
                    if (File.Exists(arquivoDestino)) File.Delete(arquivoDestino);

                    argumentosAtuais = argumentosBase.Replace("{ENCODER}", "libx264").Replace("-cq", "-crf");
                    argumentosFFmpeg = $"-y -i \"{arquivoOrigem}\" {argumentosAtuais} \"{arquivoDestino}\"";

                    sucesso = await ExecutarFFmpeg(argumentosFFmpeg, duracaoVideo, token);
                }

                cronometro.Stop();

                if (token.IsCancellationRequested)
                {
                    _atualizarLog($"[ABORTADO] Conversão cancelada pelo usuário.", Color.Red);
                    if (File.Exists(arquivoDestino)) File.Delete(arquivoDestino);
                    break;
                }

                long tamanhoFinal = File.Exists(arquivoDestino) ? new FileInfo(arquivoDestino).Length : 0;
                double reducao = tamanhoOriginal > 0 ? 100.0 * (tamanhoOriginal - tamanhoFinal) / tamanhoOriginal : 0;

                if (sucesso)
                {
                    _atualizarLog($"[SUCESSO] Salvo em: {arquivoDestino}", Color.LimeGreen);
                    if (apagarOriginais)
                    {
                        try { File.Delete(arquivoOrigem); _atualizarLog($"Original apagado: {Path.GetFileName(arquivoOrigem)}", Color.Gray); }
                        catch { _atualizarLog($"Aviso: Não foi possível apagar o original.", Color.Yellow); }
                    }
                }
                else
                {
                    _atualizarLog($"[ERRO FATAL] O FFmpeg falhou ao processar o vídeo.", Color.Red);
                }

                string statusTxt = sucesso ? "Sucesso" : "FALHA";
                string entradaLog = $"\n--------------------------------------------------\nArquivo Original: {arquivoOrigem}\nArquivo Destino:  {arquivoDestino}\nStatus:           {statusTxt}\nTamanho:          {(tamanhoOriginal / 1048576.0):F2} MB -> {(tamanhoFinal / 1048576.0):F2} MB ({reducao:F2}%)\nTempo:            {cronometro.Elapsed:mm\\:ss}";
                File.AppendAllText(arquivoLogTxt, entradaLog);

                _atualizarLog("--------------------------------------------------", Color.Gray);
            }
        }

        private TimeSpan ObterDuracaoVideo(string arquivo)
        {
            try
            {
                Process p = new Process();
                p.StartInfo.FileName = "ffmpeg.exe";
                p.StartInfo.Arguments = $"-i \"{arquivo}\"";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                string output = p.StandardError.ReadToEnd();
                p.WaitForExit();

                int durIdx = output.IndexOf("Duration: ");
                if (durIdx != -1)
                {
                    string afterDur = output.Substring(durIdx + 10).TrimStart();
                    if (afterDur.Length >= 8)
                    {
                        string durStr = afterDur.Substring(0, 8);
                        if (TimeSpan.TryParse(durStr, out TimeSpan duracao)) return duracao;
                    }
                }
            }
            catch { }
            return TimeSpan.Zero;
        }

        private string ObterMelhorEncoderGpu()
        {
            try
            {
                Process p = new Process();
                p.StartInfo.FileName = "ffmpeg.exe";
                p.StartInfo.Arguments = "-encoders";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();

                if (output.Contains("h264_nvenc")) return "h264_nvenc";
                if (output.Contains("h264_amf")) return "h264_amf";
                if (output.Contains("h264_qsv")) return "h264_qsv";
                return "libx264";
            }
            catch { return "libx264"; }
        }

        private async Task<bool> ExecutarFFmpeg(string argumentos, TimeSpan duracaoVideo, CancellationToken token)
        {
            try
            {
                Process p = new Process();
                p.StartInfo.FileName = "ffmpeg.exe";
                p.StartInfo.Arguments = argumentos;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.RedirectStandardError = true;

                p.Start();

                using (token.Register(() => { try { p.Kill(); } catch { } }))
                {
                    while (!p.StandardError.EndOfStream)
                    {
                        string linha = await p.StandardError.ReadLineAsync();

                        int timeIdx = linha.IndexOf("time=");
                        if (timeIdx != -1)
                        {
                            string afterTime = linha.Substring(timeIdx + 5).TrimStart();
                            if (afterTime.Length >= 8)
                            {
                                string tempoStr = afterTime.Substring(0, 8);
                                if (TimeSpan.TryParse(tempoStr, out TimeSpan tempoAtual))
                                {
                                    int porcentagem = 0;
                                    if (duracaoVideo.TotalSeconds > 0)
                                    {
                                        porcentagem = (int)((tempoAtual.TotalSeconds / duracaoVideo.TotalSeconds) * 100);
                                        if (porcentagem > 100) porcentagem = 100;
                                        if (porcentagem < 0) porcentagem = 0;
                                    }
                                    _atualizarProgresso(tempoStr, duracaoVideo.ToString(@"hh\:mm\:ss"), porcentagem);
                                }
                            }
                        }
                    }
                }

                p.WaitForExit();
                return p.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }
    }
}