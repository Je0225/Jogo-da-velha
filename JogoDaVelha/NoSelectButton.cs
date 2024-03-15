using System.Windows.Forms;

namespace JogoDaVelha {

    public class NoSelectButton: Button {

        public NoSelectButton() {
            SetStyle(ControlStyles.Selectable, false);
        }

    }

}