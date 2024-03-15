using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace JogoDaVelha {

    public partial class FormPrincipal: Form {

        private readonly List<Button> botoes;
        private readonly String[,] posicoes;
        private readonly String jogador; // ReadOnly só pode ser alterada dentro do construtor
        private readonly String adversario;
        private readonly Timer timer;
        private readonly Repositorio repositorio;
        private readonly Placar placar;
        private String vencedor;
        private Estado estadoAtual;

        private Boolean PrimeiroJogador { get; set; }

        public FormPrincipal() {
            InitializeComponent();
            posicoes = new String[3, 3];

            jogador = !File.Exists("X") ? "X" : "O";
            adversario = jogador.Equals("X") ? "O" : "X";
            repositorio = new Repositorio(jogador, adversario);
            repositorio.Init();

            PrimeiroJogador = jogador.Equals("X");

            vencedor = "";
            estadoAtual = Estado.AguardandoLogin;

            botoes = new List<Button>();
            botoes.AddRange(new[] { btn1, btn2, btn3, btn4, btn5, btn6, btn7, btn8, btn9 });

            placar = new Placar();
            AtualizaPlacar();

            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e) {
            timer.Stop();
            ExecutaEstado();
            timer.Start();
        }

        public void BotoesCliked(object sender, EventArgs e) {
            if (vencedor != "" || estadoAtual != Estado.MinhaVez) {
                return;
            }
            Button botao = (Button)sender;
            AcionaBotao(botao);
        }

        private void AcionaBotao(Button botao) {
            String tag = botao.Tag.ToString();
            Jogada jogada = ParseJogada(tag);
            repositorio.jogadasMinhas.Add(tag);
            repositorio.Save();

            posicoes[jogada.Linha, jogada.Coluna] = jogador;
            botao.Text = jogador;
            botao.Enabled = false;
            ChecaPosicoes();
        }

        private void Encerra(String vencedor, String[] posicoesVitoria) {
            this.vencedor = vencedor;
            estadoAtual = Estado.Encerrado;
            foreach (var botao in botoes) {
                if (botao.Tag.Equals(posicoesVitoria[0]) || botao.Tag.Equals(posicoesVitoria[1]) || botao.Tag.Equals(posicoesVitoria[2])) {
                    botao.BackColor = vencedor.Equals(jogador) ? Color.LightGreen : Color.Tomato;
                }
            }
        }

        private void ChecaPosicoes() {
            for (int i = 0; i < posicoes.GetLength(0); i++) {
                if (posicoes[i, 0] == posicoes[i, 1] && posicoes[i, 0] == posicoes[i, 2]) {
                    if (posicoes[i, 0] != null) {
                        Encerra(posicoes[i, 0], new[] { $"{i}|0", $"{i}|1", $"{i}|2" });
                    }
                } else if (posicoes[0, i] == posicoes[1, i] && posicoes[0, i] == posicoes[2, i]) {
                    if (posicoes[0, i] != null) {
                        Encerra(posicoes[0, i], new[] { $"0|{i}", $"1|{i}", $"2|{i}" });
                    }
                }
            }
            if (posicoes[0, 0] == posicoes[1, 1] && posicoes[0, 0] == posicoes[2, 2]) {
                if (posicoes[0, 0] != null) {
                    Encerra(posicoes[0, 0], new[] { "0|0", "1|1", "2|2" });
                }
            } else if (posicoes[0, 2] == posicoes[1, 1] && posicoes[1, 1] == posicoes[2, 0]) {
                if (posicoes[0, 2] != null) {
                    Encerra(posicoes[0, 2], new[] { "0|2", "1|1", "2|0" });
                }
            }
            if (estadoAtual != Estado.Encerrado) {
                Boolean faltaJogar = false;
                for (int linha = 0; linha < posicoes.GetLength(0) && !faltaJogar; linha++) {
                    for (int coluna = 0; coluna < posicoes.GetLength(1) && !faltaJogar; coluna++) {
                        if (posicoes[linha, coluna] == null) {
                            faltaJogar = true;
                        }
                    }
                }
                estadoAtual = faltaJogar ? Estado.AguardandoAdversario : Estado.Encerrado;
            }
        }

        private void ExecutaEstado() {
            if (estadoAtual.Equals(Estado.AguardandoAdversario) && !File.Exists(adversario)) {
                estadoAtual = Estado.Abandono;
            }
            switch (estadoAtual) {
                case Estado.AguardandoLogin:
                    Status("Aguardando adversário entrar no jogo", lblStatus.ForeColor.Equals(Color.White) ? Color.Firebrick : Color.White);
                    if (File.Exists(adversario) && String.IsNullOrEmpty(File.ReadAllText(adversario))) {
                        estadoAtual = PrimeiroJogador ? Estado.MinhaVez : Estado.AguardandoAdversario;
                    }
                    break;
                case Estado.AguardandoAdversario:
                    Status("Aguardando a jogada do adversário");
                    repositorio.Load();
                    if (PrimeiroJogador && repositorio.jogadasMinhas.Count == repositorio.jogadasAdversario.Count) {
                        AtualizaJogadasAdversario();
                    } else if (!PrimeiroJogador && repositorio.jogadasMinhas.Count < repositorio.jogadasAdversario.Count) {
                        AtualizaJogadasAdversario();
                    }
                    break; 
                case Estado.MinhaVez:
                    Status("Sua vez de jogar");
                    break;
                case Estado.Encerrado:
                    Status("Partida encerrada");
                    String mensagem;
                    if (vencedor != "") {
                        if (vencedor.Equals(jogador)) {
                            mensagem = "Parabéns, você venceu! :)";
                            placar.Vitoria();
                        } else {
                            mensagem = "Que pena, você perdeu :(";
                            placar.Derrota();
                        }
                    } else {
                        mensagem = "Ocorreu um empate!";
                        placar.Empate();
                    }
                    AtualizaPlacar();
                    mensagem = $"{mensagem}\n\nDeseja iniciar uma nova partida?";
                    DialogResult result = MessageBox.Show(mensagem, "Ihaaaaaaa", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                    if (result == DialogResult.Yes) {
                        ReiniciaJogo(false);
                    } else {
                        Close();
                    }
                    break;
                case Estado.Abandono:
                    MessageBox.Show("Seu adversário abandonou a partida! :(", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ReiniciaJogo(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void AtualizaJogadasAdversario() {
            foreach (var jogadaAdversario in repositorio.jogadasAdversario) {
                Jogada jogada = ParseJogada(jogadaAdversario);
                posicoes[jogada.Linha, jogada.Coluna] = adversario;

                foreach (var botao in botoes) {
                    if (botao.Tag.Equals(jogadaAdversario)) {
                        botao.Text = adversario;
                        botao.Enabled = false;
                    }
                }
            }
            ChecaPosicoes();
            if (estadoAtual != Estado.Encerrado) {
                estadoAtual = Estado.MinhaVez;
            }
        }

        private void ReiniciaJogo(Boolean HouveAbandono) {
            vencedor = "";
            for (int linha = 0; linha < posicoes.GetLength(0); linha++) {
                for (int coluna = 0; coluna < posicoes.GetLength(1); coluna++) {
                    posicoes[linha, coluna] = null;
                }
            }
            foreach (var btn in botoes) {
                btn.Text = "";
                btn.Enabled = true;
                btn.BackColor = BackColor;
            }
            repositorio.Clear();
            repositorio.Save();
            PrimeiroJogador = HouveAbandono ? jogador.Equals("X") : !PrimeiroJogador;
            estadoAtual = Estado.AguardandoLogin;
        }

        private Jogada ParseJogada(String jogada) {
            String[] partes = jogada.Split('|');
            Int32 linha = Convert.ToInt32(partes[0]);
            Int32 coluna = Convert.ToInt32(partes[1]);

            return new Jogada(linha, coluna);
        }

        private void Status(String msg, Color? cor = null) {
            lblStatus.Text = msg;
            lblStatus.ForeColor = cor ?? Color.White;
        }

        private void AtualizaPlacar() {
            lblVitorias.Text = $"Vitórias: {placar.Vitorias}";
            lblDerrotas.Text = $"Derrotas: {placar.Derrotas}";
            lblEmpates.Text = $"Empates: {placar.Empates}";
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            repositorio.Done();
        }

    }

}