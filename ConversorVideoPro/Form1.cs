using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConversorVideoPro
{
    public partial class Form1 : Form
    {
        private TextBox txtOrigem, txtDestino, txtArgumentos;
        private Button btnOrigem, btnDestino, btnConverter;
        private ComboBox cmbQualidade;
        private CheckBox chkApagar;
        private RichTextBox rtbLog, rtbGuia;

        private ProgressBar pbProgresso;

        private ComboBox cmbCriadorVideo, cmbCriadorPreset, cmbCriadorDesentrelacar;
        private ComboBox cmbCriadorCor, cmbCriadorEscala, cmbCriadorFPS;
        private ComboBox cmbCriadorAudio, cmbCriadorAudioBit, cmbCriadorCanais;
        private NumericUpDown numCriadorCRF;
        private TextBox txtCriadorFiltros, txtCriadorPreview;
        private Button btnCriadorAplicar, btnCriadorSalvar;
        private TabControl tabControl;

        public Form1()
        {
            this.Text = "Conversor de Vídeo Pro - Híbrido GPU/CPU";
            this.Size = new Size(720, 850);
            this.MinimumSize = new Size(720, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.BackColor = Color.FromArgb(245, 246, 250);
            this.Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);

            MontarDesignDaTela();
        }

        private void Form1_Load(object sender, EventArgs e) { }

        private void MontarDesignDaTela()
        {
            tabControl = new TabControl { Dock = DockStyle.Fill, ItemSize = new Size(160, 30), Padding = new Point(15, 5) };

            TabPage tabConversor = new TabPage { Text = "1. Conversor", BackColor = Color.FromArgb(245, 246, 250) };
            TabPage tabCriador = new TabPage { Text = "2. Criador de Parâmetros", BackColor = Color.White };
            TabPage tabGuia = new TabPage { Text = "3. Guia de Parâmetros", BackColor = Color.White };

            tabControl.TabPages.Add(tabConversor); tabControl.TabPages.Add(tabCriador); tabControl.TabPages.Add(tabGuia);

            // ABA 01 //
            Panel panelPastas = new Panel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(15) };
            GroupBox grpPastas = new GroupBox { Text = "1. Diretórios", Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(10) };
            TableLayoutPanel tlpPastas = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2, RowCount = 4 };
            tlpPastas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            tlpPastas.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100f));

            Label lblOrigem = new Label { Text = "Pasta de Origem:", AutoSize = true, Margin = new Padding(3, 5, 3, 2) };
            txtOrigem = new TextBox { ReadOnly = true, Dock = DockStyle.Fill, Margin = new Padding(3, 4, 3, 5) };
            btnOrigem = new Button { Text = "Procurar...", Width = 90, Height = 28, BackColor = Color.White, FlatStyle = FlatStyle.Flat, Margin = new Padding(3, 2, 3, 5) }; btnOrigem.Click += btnOrigem_Click;

            Label lblDestino = new Label { Text = "Pasta de Destino:", AutoSize = true, Margin = new Padding(3, 10, 3, 2) };
            txtDestino = new TextBox { ReadOnly = true, Dock = DockStyle.Fill, Margin = new Padding(3, 4, 3, 10) };
            btnDestino = new Button { Text = "Procurar...", Width = 90, Height = 28, BackColor = Color.White, FlatStyle = FlatStyle.Flat, Margin = new Padding(3, 2, 3, 10) }; btnDestino.Click += btnDestino_Click;

            tlpPastas.Controls.Add(lblOrigem, 0, 0); tlpPastas.Controls.Add(txtOrigem, 0, 1); tlpPastas.Controls.Add(btnOrigem, 1, 1);
            tlpPastas.Controls.Add(lblDestino, 0, 2); tlpPastas.Controls.Add(txtDestino, 0, 3); tlpPastas.Controls.Add(btnDestino, 1, 3);
            grpPastas.Controls.Add(tlpPastas); panelPastas.Controls.Add(grpPastas);

            Panel panelConfig = new Panel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(15, 0, 15, 10) };
            GroupBox grpConfig = new GroupBox { Text = "2. Configurações Básicas", Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(10) };
            TableLayoutPanel tlpConfig = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 3, RowCount = 1 };
            tlpConfig.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); tlpConfig.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f)); tlpConfig.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            Label lblQualidade = new Label { Text = "Predefinição:", AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(3, 6, 3, 5) };
            cmbQualidade = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Anchor = AnchorStyles.Left | AnchorStyles.Right, Margin = new Padding(3, 4, 15, 5) };
            cmbQualidade.Items.AddRange(new string[] { "Super Compactado (CRF 28)", "Compactado (CRF 23)", "Personalizado (Criador)" });
            chkApagar = new CheckBox { Text = "Apagar originais após sucesso", AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(3, 6, 3, 5) };

            tlpConfig.Controls.Add(lblQualidade, 0, 0); tlpConfig.Controls.Add(cmbQualidade, 1, 0); tlpConfig.Controls.Add(chkApagar, 2, 0);
            grpConfig.Controls.Add(tlpConfig); panelConfig.Controls.Add(grpConfig);

            Panel panelParametros = new Panel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(15, 0, 15, 10) };
            GroupBox grpParametros = new GroupBox { Text = "3. Parâmetros FFmpeg (Avançado)", Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(10) };
            txtArgumentos = new TextBox { Dock = DockStyle.Top, Font = new Font("Consolas", 9.5f), ForeColor = Color.DarkBlue, Margin = new Padding(3, 3, 3, 5) };
            grpParametros.Controls.Add(txtArgumentos); panelParametros.Controls.Add(grpParametros);

            cmbQualidade.SelectedIndexChanged += cmbQualidade_SelectedIndexChanged; cmbQualidade.SelectedIndex = 0;

            Panel panelBotao = new Panel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(15, 5, 15, 10) };
            TableLayoutPanel tlpBotao = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 1, RowCount = 2 };

            btnConverter = new Button { Text = "INICIAR CONVERSÃO", Height = 45, Dock = DockStyle.Fill, BackColor = Color.FromArgb(46, 204, 113), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11f, FontStyle.Bold), Cursor = Cursors.Hand };
            btnConverter.Click += btnConverter_Click;

            pbProgresso = new ProgressBar { Dock = DockStyle.Fill, Height = 10, Style = ProgressBarStyle.Continuous, Maximum = 100, Value = 0, Visible = false, Margin = new Padding(0, 5, 0, 0) };

            tlpBotao.Controls.Add(btnConverter, 0, 0);
            tlpBotao.Controls.Add(pbProgresso, 0, 1);
            panelBotao.Controls.Add(tlpBotao);

            Panel panelLog = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15, 0, 15, 15) };
            Label lblLog = new Label { Text = "Log de Processamento:", Dock = DockStyle.Top, Padding = new Padding(0, 0, 0, 5) };
            rtbLog = new RichTextBox { Dock = DockStyle.Fill, ReadOnly = true, BackColor = Color.FromArgb(30, 30, 30), ForeColor = Color.LightGray, Font = new Font("Consolas", 9f) };
            panelLog.Controls.Add(rtbLog); panelLog.Controls.Add(lblLog);

            tabConversor.Controls.Add(panelLog);
            tabConversor.Controls.Add(panelBotao);
            tabConversor.Controls.Add(panelParametros);
            tabConversor.Controls.Add(panelConfig);
            tabConversor.Controls.Add(panelPastas);

            // ABA 02 //
            Panel panelTab2Bg = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(20) }; tabCriador.Controls.Add(panelTab2Bg);

            GroupBox grpVideo = new GroupBox { Text = "Configurações de Vídeo e Imagem (-vf)", Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(10) };
            TableLayoutPanel tlpVideo = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2, RowCount = 10 };
            tlpVideo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f)); tlpVideo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));

            Label lblCVideo = new Label { Text = "Codec de Vídeo:", AutoSize = true, Margin = new Padding(3, 5, 3, 0) };
            cmbCriadorVideo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill, Margin = new Padding(3, 3, 15, 5) };
            cmbCriadorVideo.Items.AddRange(new string[] { "{ENCODER} (Automático GPU/CPU)", "libx264 (Processador H264)", "libx265 (Processador H265)", "h264_nvenc (NVIDIA)", "h264_amf (AMD)", "h264_qsv (Intel)" });

            Label lblCPreset = new Label { Text = "Preset (Velocidade):", AutoSize = true, Margin = new Padding(3, 5, 3, 0) };
            cmbCriadorPreset = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill, Margin = new Padding(3, 3, 15, 5) };
            cmbCriadorPreset.Items.AddRange(new string[] { "ultrafast", "superfast", "veryfast", "faster", "fast", "medium", "slow", "slower", "veryslow" });

            Label lblCCRF = new Label { Text = "Qualidade (CRF/CQ):", AutoSize = true, Margin = new Padding(3, 5, 3, 0) };
            numCriadorCRF = new NumericUpDown { Dock = DockStyle.Fill, Margin = new Padding(3, 3, 15, 5), Minimum = 0, Maximum = 51, Value = 23 };

            Label lblCDesentrelacar = new Label { Text = "Desentrelaçamento:", AutoSize = true, Margin = new Padding(3, 5, 3, 0) };
            cmbCriadorDesentrelacar = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill, Margin = new Padding(3, 3, 15, 5) };
            cmbCriadorDesentrelacar.Items.AddRange(new string[] { "Nenhum", "yadif (Padrão/Seguro)", "bwdif (Rápido/Moderno)" });

            Label lblCCor = new Label { Text = "Espaço de Cor (Pixel Format):", AutoSize = true, Margin = new Padding(3, 5, 3, 0) };
            cmbCriadorCor = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill, Margin = new Padding(3, 3, 15, 5) };
            cmbCriadorCor.Items.AddRange(new string[] { "Original (Manter)", "format=yuv420p (Padrão/Seguro)" });

            Label lblCEscala = new Label { Text = "Redimensionar (Escala):", AutoSize = true, Margin = new Padding(3, 5, 3, 0) };
            cmbCriadorEscala = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill, Margin = new Padding(3, 3, 15, 5) };
            cmbCriadorEscala.Items.AddRange(new string[] { "Original (Manter resolução)", "scale=1920:1080 (Forçar 1080p)", "scale=1280:-2 (Forçar 720p sem esticar)" });

            Label lblCFPS = new Label { Text = "Framerate (FPS):", AutoSize = true, Margin = new Padding(3, 5, 3, 0) };
            cmbCriadorFPS = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill, Margin = new Padding(3, 3, 15, 5) };
            cmbCriadorFPS.Items.AddRange(new string[] { "Original (Manter)", "-r 23.976", "-r 24", "-r 29.97", "-r 30", "-r 59.94", "-r 60" });

            Label lblCFiltros = new Label { Text = "Filtros Extras Manuais (Opcional):", AutoSize = true, Margin = new Padding(3, 5, 3, 0) };
            txtCriadorFiltros = new TextBox { Text = "", Dock = DockStyle.Fill, Margin = new Padding(3, 3, 15, 10) };

            tlpVideo.Controls.Add(lblCVideo, 0, 0); tlpVideo.Controls.Add(lblCPreset, 1, 0);
            tlpVideo.Controls.Add(cmbCriadorVideo, 0, 1); tlpVideo.Controls.Add(cmbCriadorPreset, 1, 1);
            tlpVideo.Controls.Add(lblCCRF, 0, 2); tlpVideo.Controls.Add(lblCDesentrelacar, 1, 2);
            tlpVideo.Controls.Add(numCriadorCRF, 0, 3); tlpVideo.Controls.Add(cmbCriadorDesentrelacar, 1, 3);
            tlpVideo.Controls.Add(lblCCor, 0, 4); tlpVideo.Controls.Add(lblCEscala, 1, 4);
            tlpVideo.Controls.Add(cmbCriadorCor, 0, 5); tlpVideo.Controls.Add(cmbCriadorEscala, 1, 5);

            tlpVideo.Controls.Add(lblCFPS, 0, 6);
            tlpVideo.Controls.Add(cmbCriadorFPS, 0, 7);

            tlpVideo.Controls.Add(lblCFiltros, 0, 8); tlpVideo.SetColumnSpan(lblCFiltros, 2);
            tlpVideo.Controls.Add(txtCriadorFiltros, 0, 9); tlpVideo.SetColumnSpan(txtCriadorFiltros, 2);
            grpVideo.Controls.Add(tlpVideo);

            Panel gap1 = new Panel { Dock = DockStyle.Top, Height = 15 };

            GroupBox grpAudio = new GroupBox { Text = "Configurações de Áudio", Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(10) };
            TableLayoutPanel tlpAudio = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2, RowCount = 4 };
            tlpAudio.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f)); tlpAudio.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));

            Label lblCAudio = new Label { Text = "Codec de Áudio:", AutoSize = true, Margin = new Padding(3, 5, 3, 0) };
            cmbCriadorAudio = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill, Margin = new Padding(3, 3, 15, 5) };
            cmbCriadorAudio.Items.AddRange(new string[] { "aac", "mp3", "copy" });

            Label lblCBitrate = new Label { Text = "Bitrate de Áudio:", AutoSize = true, Margin = new Padding(3, 5, 3, 0) };
            cmbCriadorAudioBit = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill, Margin = new Padding(3, 3, 15, 5) };
            cmbCriadorAudioBit.Items.AddRange(new string[] { "128k", "192k", "256k", "320k" });

            Label lblCCanais = new Label { Text = "Canais de Áudio (Mixagem):", AutoSize = true, Margin = new Padding(3, 5, 3, 0) };
            cmbCriadorCanais = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill, Margin = new Padding(3, 3, 15, 10) };
            cmbCriadorCanais.Items.AddRange(new string[] { "Original (Não alterar)", "Stereo (2 Canais - Mais compatível)", "Mono (1 Canal)" });

            tlpAudio.Controls.Add(lblCAudio, 0, 0); tlpAudio.Controls.Add(lblCBitrate, 1, 0);
            tlpAudio.Controls.Add(cmbCriadorAudio, 0, 1); tlpAudio.Controls.Add(cmbCriadorAudioBit, 1, 1);
            tlpAudio.Controls.Add(lblCCanais, 0, 2);
            tlpAudio.Controls.Add(cmbCriadorCanais, 0, 3);
            grpAudio.Controls.Add(tlpAudio);

            Panel gap2 = new Panel { Dock = DockStyle.Top, Height = 20 };
            Label lblCPreview = new Label { Text = "Pré-visualização do Comando Gerado:", Dock = DockStyle.Top, Padding = new Padding(0, 0, 0, 5) };
            txtCriadorPreview = new TextBox { Dock = DockStyle.Top, Font = new Font("Consolas", 9.5f), ForeColor = Color.DarkBlue, ReadOnly = true };
            Panel gap3 = new Panel { Dock = DockStyle.Top, Height = 20 };

            TableLayoutPanel tlpBotoesCriador = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2 };
            tlpBotoesCriador.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f)); tlpBotoesCriador.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            btnCriadorAplicar = new Button { Text = "APLICAR NO CONVERSOR", Height = 40, Dock = DockStyle.Top, BackColor = Color.FromArgb(52, 152, 219), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10f, FontStyle.Bold), Margin = new Padding(0, 0, 10, 0) }; btnCriadorAplicar.Click += btnCriadorAplicar_Click;
            btnCriadorSalvar = new Button { Text = "SALVAR PREDEFINIÇÃO", Height = 40, Dock = DockStyle.Top, BackColor = Color.FromArgb(149, 165, 166), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10f, FontStyle.Bold), Margin = new Padding(10, 0, 0, 0) }; btnCriadorSalvar.Click += btnCriadorSalvar_Click;
            tlpBotoesCriador.Controls.Add(btnCriadorAplicar, 0, 0); tlpBotoesCriador.Controls.Add(btnCriadorSalvar, 1, 0);

            panelTab2Bg.Controls.Add(tlpBotoesCriador); panelTab2Bg.Controls.Add(gap3); panelTab2Bg.Controls.Add(txtCriadorPreview); panelTab2Bg.Controls.Add(lblCPreview); panelTab2Bg.Controls.Add(gap2); panelTab2Bg.Controls.Add(grpAudio); panelTab2Bg.Controls.Add(gap1); panelTab2Bg.Controls.Add(grpVideo);

            cmbCriadorVideo.SelectedIndex = 0; cmbCriadorPreset.SelectedIndex = 5; cmbCriadorDesentrelacar.SelectedIndex = 0;
            cmbCriadorCor.SelectedIndex = 1; cmbCriadorEscala.SelectedIndex = 0; cmbCriadorFPS.SelectedIndex = 0;
            cmbCriadorAudio.SelectedIndex = 0; cmbCriadorAudioBit.SelectedIndex = 2; cmbCriadorCanais.SelectedIndex = 1;

            cmbCriadorVideo.SelectedIndexChanged += AtualizarPreviewCriador; cmbCriadorPreset.SelectedIndexChanged += AtualizarPreviewCriador;
            cmbCriadorDesentrelacar.SelectedIndexChanged += AtualizarPreviewCriador; cmbCriadorCor.SelectedIndexChanged += AtualizarPreviewCriador;
            cmbCriadorEscala.SelectedIndexChanged += AtualizarPreviewCriador; cmbCriadorFPS.SelectedIndexChanged += AtualizarPreviewCriador;
            cmbCriadorAudio.SelectedIndexChanged += AtualizarPreviewCriador; cmbCriadorAudioBit.SelectedIndexChanged += AtualizarPreviewCriador;
            cmbCriadorCanais.SelectedIndexChanged += AtualizarPreviewCriador;
            numCriadorCRF.ValueChanged += AtualizarPreviewCriador; txtCriadorFiltros.TextChanged += AtualizarPreviewCriador;

            AtualizarPreviewCriador(null, null);

            // ABA 03 //
            rtbGuia = new RichTextBox { Dock = DockStyle.Fill, ReadOnly = true, BackColor = Color.White, BorderStyle = BorderStyle.None, Padding = new Padding(20) };
            ConstruirTextoDoGuia(); tabGuia.Controls.Add(rtbGuia);

            this.Controls.Add(tabControl);
        }

        private void AtualizarPreviewCriador(object sender, EventArgs e)
        {
            if (cmbCriadorVideo.SelectedItem == null) return;
            string videoCodec = cmbCriadorVideo.SelectedItem.ToString().Split(' ')[0];
            string preset = cmbCriadorPreset.SelectedItem.ToString();
            string crf = numCriadorCRF.Value.ToString();
            string fps = cmbCriadorFPS.SelectedItem.ToString();
            string audioCodec = cmbCriadorAudio.SelectedItem.ToString();
            string audioBit = cmbCriadorAudioBit.SelectedItem.ToString();

            string desentrelacar = cmbCriadorDesentrelacar.SelectedItem.ToString().Split(' ')[0];
            string cor = cmbCriadorCor.SelectedItem.ToString().Split(' ')[0];
            string escala = cmbCriadorEscala.SelectedItem.ToString().Split(' ')[0];
            string canais = cmbCriadorCanais.SelectedItem.ToString();

            string preview = $"-c:v {videoCodec}";
            if (videoCodec.Contains("nvenc") || videoCodec.Contains("amf") || videoCodec.Contains("qsv") || videoCodec == "{ENCODER}") { preview += $" -cq {crf} -preset {preset}"; }
            else { preview += $" -crf {crf} -preset {preset}"; }

            if (!fps.StartsWith("Original"))
            {
                preview += $" {fps}";
            }

            List<string> filtrosAtivos = new List<string>();
            if (desentrelacar != "Nenhum") filtrosAtivos.Add(desentrelacar);
            if (cor != "Original") filtrosAtivos.Add(cor);
            if (escala != "Original") filtrosAtivos.Add(escala);
            if (!string.IsNullOrWhiteSpace(txtCriadorFiltros.Text)) filtrosAtivos.Add(txtCriadorFiltros.Text);

            if (filtrosAtivos.Count > 0)
            {
                string filtrosJuntos = string.Join(",", filtrosAtivos);
                preview += $" -vf \"{filtrosJuntos}\"";
            }

            preview += $" -c:a {audioCodec}";
            if (audioCodec != "copy") preview += $" -b:a {audioBit}";

            if (canais.StartsWith("Stereo")) preview += " -ac 2";
            else if (canais.StartsWith("Mono")) preview += " -ac 1";

            txtCriadorPreview.Text = preview;
        }

        private void btnCriadorAplicar_Click(object sender, EventArgs e) { txtArgumentos.Text = txtCriadorPreview.Text; cmbQualidade.SelectedIndex = 2; tabControl.SelectedIndex = 0; }
        private void btnCriadorSalvar_Click(object sender, EventArgs e) { MessageBox.Show("O código visual não processa salvamentos. Será implementado na lógica futuramente.", "Visual Apenas", MessageBoxButtons.OK, MessageBoxIcon.Information); }

        private void cmbQualidade_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbQualidade.SelectedIndex == 0) txtArgumentos.Text = "-c:v {ENCODER} -cq 28 -preset slow -vf \"format=yuv420p\" -c:a aac -b:a 256k -ac 2";
            else if (cmbQualidade.SelectedIndex == 1) txtArgumentos.Text = "-c:v {ENCODER} -cq 23 -preset medium -vf \"format=yuv420p\" -c:a aac -b:a 320k -ac 2";
        }

        private void btnOrigem_Click(object sender, EventArgs e) { using (FolderBrowserDialog fbd = new FolderBrowserDialog { Description = "Origem" }) if (fbd.ShowDialog() == DialogResult.OK) txtOrigem.Text = fbd.SelectedPath; }
        private void btnDestino_Click(object sender, EventArgs e) { using (FolderBrowserDialog fbd = new FolderBrowserDialog { Description = "Destino" }) if (fbd.ShowDialog() == DialogResult.OK) txtDestino.Text = fbd.SelectedPath; }

        private void ConstruirTextoDoGuia()
        {
            rtbGuia.SelectionFont = new Font("Segoe UI", 16f, FontStyle.Bold);
            rtbGuia.AppendText("Guia Avançado de Parâmetros (FFmpeg)\n\n");

            rtbGuia.SelectionFont = new Font("Segoe UI", 10f, FontStyle.Regular);
            rtbGuia.AppendText("O FFmpeg é a ferramenta mais poderosa do mundo para processamento de mídia. Abaixo, detalhamos os comandos aceitos por este aplicativo para que você possa extrair o máximo do seu hardware.\n\n");

            AdicionarItemGuia("{ENCODER}", "Tag Inteligente Exclusiva", "Deixe esta tag no lugar do codec de vídeo. O programa fará uma varredura no seu PC e escolherá automaticamente a Placa de Vídeo. Caso o seu PC não tenha GPU compatível ou ocorra um erro de renderização pesada (como formatos ProRes), ele fará o fallback automático para o processador (libx264) sem que a conversão falhe.");

            AdicionarItemGuia("-c:v", "Codec de Vídeo (Video Codec)", "Define o 'motor' que fará a compactação da imagem.\n• libx264: Usado pelo processador (CPU). Maior compatibilidade do mundo, mas exige 100% da sua máquina.\n• libx265: Usado pelo processador. Gera arquivos 50% menores que o h264 com a mesma qualidade, mas demora o dobro do tempo para renderizar.\n• h264_nvenc / hevc_nvenc: Aceleração via NVIDIA. Extremamente rápido, ideal para lotes gigantes de vídeos.\n• h264_amf / hevc_amf: Aceleração via AMD (Radeon).\n• h264_qsv: Aceleração via placa Intel.");

            AdicionarItemGuia("-crf / -cq", "Qualidade Visual Constante", "Define a qualidade do vídeo. Diferente do Bitrate tradicional, ele aloca dados dinamicamente (cenas escuras usam menos dados, cenas de explosão usam mais).\n• 18 a 20: Visualmente idêntico ao original (Arquivo grande).\n• 23: Qualidade Padrão / Recomendado (Ótimo custo-benefício).\n• 28 a 30: Compressão agressiva (Ideal para enviar no WhatsApp ou arquivar).");

            AdicionarItemGuia("-preset", "Velocidade de Processamento", "Determina o esforço que o computador fará para encontrar formas de comprimir o vídeo. No processador, segue os nomes abaixo:\n• ultrafast / superfast: Muito rápido, mas o arquivo final ficará maior para compensar a pressa.\n• medium: O equilíbrio perfeito (Padrão).\n• slow / slower / veryslow: Demora muito, mas entrega um arquivo super pequeno mantendo a qualidade altíssima.");

            AdicionarItemGuia("-r", "Framerate (Taxa de Quadros)", "Força a conversão para uma taxa de quadros específica. Ajuda a padronizar arquivos de celular (que gravam em taxa variável) para ilhas de edição.\n• -r 23.976 / -r 24: Padrão de Cinema.\n• -r 29.97 / -r 30: Padrão NTSC (TV/DVD e Web).\n• -r 59.94 / -r 60: Movimento super fluido (Games, Drones e Web).");

            AdicionarItemGuia("-vf", "Video Filters (Filtros Visuais Complexos)", "Manipulação direta na imagem do vídeo. Eles devem ser separados por vírgula (,).\n• yadif ou bwdif: Desentrelaça o vídeo. Fundamental para vídeos de TV ou XDCAM antigas. Remove o aspecto de 'linhas cortadas' em movimento.\n• format=yuv420p: Força a tabela de cores padrão do H264 (Obrigatório para Apple ProRes, senão a tela fica preta no celular).\n• scale=1920:1080: Força a resolução para Full HD.\n• scale=1280:-2: Força a largura para 720p e calcula a altura sozinho para não esticar a imagem.");

            AdicionarItemGuia("-c:a / -b:a", "Codec e Bitrate de Áudio", "Processamento da faixa de som.\n• -c:a aac: Melhor formato de áudio para MP4.\n• -c:a copy: Apenas copia o original sem reconverter (útil para ganhar tempo).\n• -b:a 128k: Qualidade padrão de rádio e YouTube (128 kbps).\n• -b:a 256k / 320k: Áudio em altíssima resolução (Qualidade de CD).");

            AdicionarItemGuia("-ac", "Audio Channels (Mixagem/Downmix)", "Junta e reduz os canais de áudio. Muito importante para vídeos profissionais que possuem 4, 8 ou 16 canais de áudio separados. Sem isso, os reprodutores de MP4 padrão podem omitir vozes e efeitos.\n• -ac 2: Mixa todos os canais para Stereo (Padrão seguro para qualquer player).\n• -ac 1: Mixa tudo para Mono.");
        }

        private void AdicionarItemGuia(string cmd, string tit, string desc)
        {
            rtbGuia.SelectionFont = new Font("Consolas", 12f, FontStyle.Bold); rtbGuia.SelectionColor = Color.DarkRed; rtbGuia.AppendText(cmd + " ");
            rtbGuia.SelectionFont = new Font("Segoe UI", 11f, FontStyle.Bold); rtbGuia.SelectionColor = Color.FromArgb(50, 50, 50); rtbGuia.AppendText("- " + tit + "\n");
            rtbGuia.SelectionFont = new Font("Segoe UI", 10f, FontStyle.Regular); rtbGuia.SelectionColor = Color.Black; rtbGuia.AppendText(desc + "\n\n");
        }

            // MOTOR //
        private CancellationTokenSource _tokenCancelamento;
        private bool _processando = false;

        private void AtualizarLog(string mensagem, Color cor)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => AtualizarLog(mensagem, cor)));
                return;
            }
            rtbLog.SelectionStart = rtbLog.TextLength;
            rtbLog.SelectionLength = 0;
            rtbLog.SelectionColor = cor;
            rtbLog.AppendText(mensagem + "\n");
            rtbLog.ScrollToCaret();
        }

        private void AtualizarProgresso(string tempoAtual, string tempoTotal, int porcentagem)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => AtualizarProgresso(tempoAtual, tempoTotal, porcentagem)));
                return;
            }

            if (_processando)
            {
                pbProgresso.Visible = true;
                pbProgresso.Value = porcentagem;
                btnConverter.Text = $"ABORTAR CONVERSÃO ({porcentagem}%  |  {tempoAtual} / {tempoTotal})";
            }
        }

        private async void btnConverter_Click(object sender, EventArgs e)
        {
            if (_processando)
            {
                DialogResult confirmacao = MessageBox.Show("Deseja realmente abortar a conversão em andamento?", "Abortar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirmacao == DialogResult.Yes)
                {
                    _tokenCancelamento?.Cancel();
                    btnConverter.Enabled = false;
                    btnConverter.Text = "ABORTANDO PROCESSO...";
                }
                return;
            }

            if (string.IsNullOrWhiteSpace(txtOrigem.Text) || string.IsNullOrWhiteSpace(txtDestino.Text))
            {
                MessageBox.Show("Selecione as pastas de origem e destino.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
            }

            _processando = true;
            _tokenCancelamento = new CancellationTokenSource();

            btnConverter.BackColor = Color.FromArgb(231, 76, 60);
            btnConverter.Text = "ABORTAR CONVERSÃO (Calculando tempo...)";
            rtbLog.Clear();

            AtualizarLog($"[{DateTime.Now:HH:mm:ss}] Argumentos Base: {txtArgumentos.Text}", Color.White);
            AtualizarLog($"[{DateTime.Now:HH:mm:ss}] Iniciando varredura na pasta...\n", Color.LightBlue);

            MotorDeConversao motor = new MotorDeConversao();

            try
            {
                await Task.Run(async () => {
                    await motor.IniciarProcessoAsync(
                        txtOrigem.Text,
                        txtDestino.Text,
                        txtArgumentos.Text,
                        chkApagar.Checked,
                        this.AtualizarLog,
                        this.AtualizarProgresso,
                        _tokenCancelamento.Token
                    );
                });
            }
            finally
            {
                _processando = false;
                _tokenCancelamento?.Dispose();
                pbProgresso.Visible = false;
                pbProgresso.Value = 0;

                AtualizarLog($"\n[{DateTime.Now:HH:mm:ss}] FIM DA TAREFA.", Color.LimeGreen);
                btnConverter.Enabled = true;
                btnConverter.Text = "INICIAR CONVERSÃO";
                btnConverter.BackColor = Color.FromArgb(46, 204, 113);
            }
        }
    }
}