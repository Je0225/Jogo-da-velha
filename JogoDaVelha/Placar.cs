using System;

namespace JogoDaVelha {

    public class Placar {

        public Int32 Vitorias { get; private set; }
        public Int32 Derrotas { get; private set; }
        public Int32 Empates { get; private set; }

        public void Vitoria() {
            Vitorias += 1;
        }

        public void Derrota() {
            Derrotas += 1;
        }

        public void Empate() {
            Empates += 1;
        }

    }

}