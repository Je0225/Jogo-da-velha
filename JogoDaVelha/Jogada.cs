using System;

namespace JogoDaVelha {

    public struct Jogada {

        public Int32 Linha { get; }
        public Int32 Coluna { get;}

        public Jogada(Int32 linha, Int32 coluna) {
            Linha = linha;
            Coluna = coluna;
        }

    }

}