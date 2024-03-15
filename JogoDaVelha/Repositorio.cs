using System;
using System.Collections.Generic;
using System.IO;

namespace JogoDaVelha {

    public class Repositorio {

        public readonly List<String> jogadasMinhas;
        public readonly List<String> jogadasAdversario;
        public String Jogador { get; }
        public String Adversario { get; }

        public Repositorio(String jogador, String adversario) {
            jogadasMinhas = new List<String>();
            jogadasAdversario = new List<String>();
            Jogador = jogador;
            Adversario = adversario;
        }

        public void Init() {
            File.WriteAllText(Jogador, "");
        }

        public void Save() {
            File.WriteAllLines(Jogador, jogadasMinhas);
        }

        public void Load() {
            try {
                String[] jogadas = File.ReadAllLines(Adversario);
                jogadasAdversario.Clear();
                if (File.Exists(Adversario)) {
                    jogadasAdversario.AddRange(jogadas);
                }
            } catch {
                // qualquer coisa
            }
        }

        public void Clear() {
            jogadasMinhas.Clear();
            jogadasAdversario.Clear();
        }

        public void Done() {
            File.Delete(Jogador);
        }

    }

}