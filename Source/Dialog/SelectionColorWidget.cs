using UnityEngine;

namespace ReColorStockpile.Dialog
{
    delegate void SelectionChangeListener(object sender);

    class SelectionColorWidget
    {
        public event SelectionChangeListener SelectionChangeListener;

        public readonly Color OriginalColor;

        private Color selectedColor;

        public Color SelectedColor
        {
            get { return this.selectedColor; }
            set
            {
                if (!this.selectedColor.Equals(value))
                {
                    this.selectedColor = value;
                    this.SelectionChangeListener?.Invoke(this);
                }
            }
        }

        public SelectionColorWidget(Color color)
        {
            this.OriginalColor = color;
            this.selectedColor = color;
        }

        public void ResetToDefault()
        {
            this.selectedColor = this.OriginalColor;
        }
    }
}
