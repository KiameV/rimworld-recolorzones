using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Verse;

namespace ReColorStockpile.Dialog
{
    class ColorSelectDialog : Window
    {
        internal static Texture2D ChangeColorTexture;
        internal static Texture2D ColorPickerTexture;
        private static readonly Texture2D ColorPresetTexture = new Texture2D(20, 20);

        private readonly ColorPresets ColorPresets;

        private readonly SelectionColorWidget SelectionColorWidget;
        private Zone stockpile;

        public ColorSelectDialog(Zone_Stockpile stockpile) : base()
        {
            if (ColorPresets == null)
            {
                ColorPresets = IOUtil.LoadColorPresets();
            }
            this.stockpile = stockpile;
            this.SelectionColorWidget = new SelectionColorWidget(stockpile.color);
            this.SelectionColorWidget.SelectionChangeListener += this.updateMaterial;
        }

        public override Vector2 InitialSize
        {
            get
            {
                return new Vector2(400, 550);
            }
        }

        public override void PreClose()
        {
            base.PreClose();
            if (ColorPresets.IsModified)
            {
                IOUtil.SaveColorPresets(ColorPresets);
                ColorPresets.IsModified = false;
            }
        }
        public override void DoWindowContents(Rect inRect)
        {
            StringBuilder sb = new StringBuilder(this.stockpile.label);
            sb.Append(" ");
            sb.Append("ReColorStockpile.Color".Translate());
            GUI.Label(new Rect(5, 5, inRect.width - 10, 25), sb.ToString());
            List<SelectionColorWidget> l = new List<SelectionColorWidget>(1);
            l.Add(this.SelectionColorWidget);
            this.AddColorSelectorWidget(15, 10, inRect.width - 30, l, this.ColorPresets);
            
            Text.Font = GameFont.Small;
            if (Widgets.ButtonText(new Rect(inRect.width * 0.5f - 30, inRect.height - 30, 60, 30), "CloseButton".Translate()))
            {
                base.Close();
            }
        }

        public void AddColorSelectorWidget(float left, float top, float width, List<SelectionColorWidget> selectionDtos, ColorPresets presetsDto)
        {
            Text.Font = GameFont.Medium;

            Rect colorPickerRect = new Rect(0, 25f, width, ColorPickerTexture.height * width / ColorPickerTexture.width);

            GUI.BeginGroup(new Rect(left, top, width, colorPickerRect.height + 20));
            GUI.color = Color.white;
            if (GUI.RepeatButton(colorPickerRect, ColorPickerTexture, GUI.skin.label))
            {
                SetColorToSelected(selectionDtos, presetsDto, GetColorFromTexture(Event.current.mousePosition, colorPickerRect, ColorPickerTexture));
            }
            GUI.EndGroup();

            Color rgbColor = Color.white;
            if (presetsDto.HasSelected())
            {
                rgbColor = presetsDto.GetSelectedColor();
            }
            else if (selectionDtos.Count > 0)
            {
                rgbColor = selectionDtos[0].SelectedColor;
            }

            GUI.BeginGroup(new Rect(0, colorPickerRect.height + 50f, width, 30f));
            GUI.Label(new Rect(0, 0, 75, 40), "ReColorStockpile.Alpha".Translate());
            rgbColor.a = Widgets.HorizontalSlider(new Rect(90, 0, 150, 20), rgbColor.a, 0, 1);
            GUI.EndGroup();

            GUI.BeginGroup(new Rect(0, colorPickerRect.height + 90f, width, 30f));
            GUI.Label(new Rect(0f, 0f, 10f, 20f), "R");
            string rText = GUI.TextField(new Rect(12f, 1f, 30f, 20f), ColorConvert(rgbColor.r), 3);

            GUI.Label(new Rect(52f, 0f, 10f, 20f), "G");
            string gText = GUI.TextField(new Rect(64f, 1f, 30f, 20f), ColorConvert(rgbColor.g), 3);

            GUI.Label(new Rect(104f, 0f, 10f, 20f), "B");
            string bText = GUI.TextField(new Rect(116f, 1f, 30f, 20f), ColorConvert(rgbColor.b), 3);

            GUI.Label(new Rect(156f, 0f, 10f, 20f), "A");
            string aText = GUI.TextField(new Rect(168f, 1f, 30f, 20f), ColorConvert(rgbColor.a), 3);
            GUI.EndGroup();

            GUI.BeginGroup(new Rect(0, colorPickerRect.height + 130, width, 120));
            GUI.Label(new Rect(0, 0, 100, 30), "Presets:");
            bool skipRGB = false;
            float l = 0;
            for (int i = 0; i < presetsDto.Count; ++i)
            {
                GUI.color = presetsDto[i];
                l += ColorPresetTexture.width + 4;
                Rect presetRect = new Rect(l, 32, ColorPresetTexture.width, ColorPresetTexture.height);
                GUI.Label(presetRect, new GUIContent(ColorPresetTexture, "ReColorStockpile.ColorPresetHelp".Translate()));
                if (Widgets.ButtonInvisible(presetRect, false))
                {
                    if (Event.current.shift)
                    {
                        if (presetsDto.IsSelected(i))
                        {
                            presetsDto.Deselect();
                        }
                        else
                        {
                            if (selectionDtos.Count > 0 &&
                                !presetsDto.HasSelected())
                            {
                                presetsDto.SetColor(i, selectionDtos[0].SelectedColor);
                            }
                            presetsDto.SetSelected(i);
                        }
                    }
                    else
                    {
                        SetColorToSelected(selectionDtos, null, presetsDto[i]);
                    }
                    skipRGB = true;
                }
                GUI.color = Color.white;
                if (presetsDto.IsSelected(i))
                {
                    Widgets.DrawBox(presetRect, 1);
                }
            }
            GUI.Label(new Rect(0, 30 + ColorPresetTexture.height + 2, width, 60), GUI.tooltip);
            GUI.EndGroup();

            if (!skipRGB &&
                (selectionDtos.Count > 0 || presetsDto.HasSelected()))
            {
                Color c = Color.white;
                c.r = ColorConvert(rText);
                c.g = ColorConvert(gText);
                c.b = ColorConvert(bText);
                c.a = ColorConvert(aText);

                SetColorToSelected(selectionDtos, presetsDto, c);
            }
        }

        private static void SetColorToSelected(List<SelectionColorWidget> dtos, ColorPresets presetsDto, Color color)
        {
            if (presetsDto != null && presetsDto.HasSelected())
            {
                presetsDto.SetSelectedColor(color);
            }
            else if (dtos.Count > 0)
            {
                foreach (SelectionColorWidget dto in dtos)
                {
                    dto.SelectedColor = color;
                }
            }
        }
        
        private FieldInfo materialIntInfo = null;
        private void updateMaterial(object sender)
        {
            this.stockpile.color = this.SelectionColorWidget.SelectedColor;
            if (materialIntInfo == null)
            {
                materialIntInfo = typeof(Zone).GetField("materialInt", BindingFlags.NonPublic | BindingFlags.Instance);
            }

            materialIntInfo.SetValue(this.stockpile, null);

            MapDrawer mapDrawer = Find.VisibleMap.mapDrawer;
            foreach (IntVec3 cell in this.stockpile.cells)
            {
                mapDrawer.SectionAt(cell).RegenerateAllLayers();
            }
        }

        private Color GetColorFromTexture(Vector2 mousePosition, Rect rect, Texture2D texture)
        {
            float localMouseX = mousePosition.x - rect.x;
            float localMouseY = mousePosition.y - rect.y;
            int imageX = (int)(localMouseX * ((float)ColorPickerTexture.width / (rect.width + 0f)));
            int imageY = (int)((rect.height - localMouseY) * ((float)ColorPickerTexture.height / (rect.height + 0f)));
            Color pixel = texture.GetPixel(imageX, imageY);
            pixel.a = (ColorPresets.HasSelected()) ? ColorPresets.GetSelectedColor().a : this.SelectionColorWidget.SelectedColor.a;
            return pixel;
        }

        private string ColorConvert(float f)
        {
            try
            {
                int i = (int)(f * 255);
                if (i > 255)
                {
                    i = 255;
                }
                else if (i < 0)
                {
                    i = 0;
                }
                return i.ToString();
            }
            catch
            {
                return "0";
            }
        }

        private float ColorConvert(string intText)
        {
            try
            {
                float f = int.Parse(intText) / 255f;
                if (f > 1)
                {
                    f = 1;
                }
                else if (f < 0)
                {
                    f = 0;
                }
                return f;
            }
            catch
            {
                return 0;
            }

        }
    }
}
