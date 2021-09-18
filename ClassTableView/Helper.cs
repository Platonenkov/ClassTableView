using System.Collections.Generic;
using DocumentFormat.OpenXml.Spreadsheet;
using OpenXmlEx.Styles;
using OpenXmlEx.Styles.Base;
using Color = System.Drawing.Color;

namespace ClassTableView
{
    public static class Helper
    {
        public static OpenXmlExStyles Styles = new(
            new List<BaseOpenXmlExStyle>()
            {
                // Стиль для заголовков (№2)
                new()
                {
                    FontColor = System.Drawing.Color.Black,
                    FontSize = 12,
                    IsBoldFont = true,
                    LeftBorderStyle = BorderStyleValues.Thick,
                    RightBorderStyle = BorderStyleValues.Thick,
                    BottomBorderStyle = BorderStyleValues.Thick,
                    BorderColor = System.Drawing.Color.Black,
                    FillColor = Color.Orange, FillPattern = PatternValues.Solid
                },
                // Стиль для заголовков (№3)
                new()
                {
                    FontColor = System.Drawing.Color.Black,
                    FontSize = 12,
                    IsBoldFont = true,
                    LeftBorderStyle = BorderStyleValues.Thick,
                    RightBorderStyle = BorderStyleValues.Thick,
                    BottomBorderStyle = BorderStyleValues.Thick,
                    BorderColor = System.Drawing.Color.Black,
                    FillColor = Color.LightSteelBlue, FillPattern = PatternValues.Solid
                },
                // стиль для основных данных (№4)
                new()
                {
                    FontSize = 12,
                    LeftBorderStyle = BorderStyleValues.Dashed,
                    RightBorderStyle = BorderStyleValues.Dashed,
                    BorderColor = System.Drawing.Color.Black
                }
            });
    }
}