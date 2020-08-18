using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace RWGame
{
    public class GuideStep
    {
        public View View { get; set; }
        public string guidePhrase { get; set; }
        public GuideStep(View _button, string _guidePhrase)
        {
            View = _button;
            guidePhrase = _guidePhrase;
        }
    }
    public class TourGuide
    {
        string guidePhrase;
        View tempView;
        int widening = 2;
        double dpi = DeviceDisplay.MainDisplayInfo.Density;
        bool isGuideActive = true;

        SKCanvasView canvasView;
        private int countTapped = 0;
        List<GuideStep> guide;
        TapGestureRecognizer localCanvasTappedRecognizer;
        public TourGuide(SKCanvasView canvasView)
        {
            this.canvasView = canvasView;
            canvasView.PaintSurface += OnCanvasViewPaintSurface;

        }
        public void StartIntroGuide(List<GuideStep> guide)
        {
            this.guide = guide;
            countTapped = 1;
            isGuideActive = true;
            canvasView.IsVisible = true;
            canvasView.IsEnabled = true;
            PerformGuideStep(guide[0]);
            localCanvasTappedRecognizer = new TapGestureRecognizer();
            canvasView.GestureRecognizers.Add(localCanvasTappedRecognizer);
            localCanvasTappedRecognizer.Tapped += TapGuide;
        }

        private void TapGuide(Object obj, Object args)
        {
            if (countTapped<guide.Count && guide[countTapped] != null)
            {
                PerformGuideStep(guide[countTapped]);
                countTapped++;
            }
            else
            {
                canvasView.IsVisible = false;
                canvasView.IsEnabled = false;
                countTapped = 1;
                localCanvasTappedRecognizer.Tapped -= TapGuide;
                return;
            }
        }

        void PerformGuideStep(GuideStep step)
        {
            tempView = step.View;
            guidePhrase = step.guidePhrase;
            canvasView.InvalidateSurface();
        }
        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;
            canvas.Clear();
            //float vx = GetAbsoluteX(canvasView);
            float vy = GetAbsoluteY(canvasView);
            if (isGuideActive)
            {
                if (tempView != null)
                {
                    float x = GetAbsoluteX(tempView);
                    float y = GetAbsoluteY(tempView);
                    Rectangle rect = new Rectangle( // White rectangle around a clickable item
                        DpToPx(x - widening),
                        DpToPx(y - vy - widening),
                        DpToPx(tempView.Width + widening * 2),
                        DpToPx(tempView.Height + widening));
                    Darken(canvas, rect);
                    DrawLightRect(canvas, rect);
                    DisplayText(canvas, guidePhrase, rect);
                    
                }
                else
                {
                    float x = DpToPx(App.ScreenWidth / 2);
                    float y = DpToPx(App.ScreenHeight / 2);
                    float margin = 15;
                    float width = DpToPx(App.ScreenWidth - 2 * margin);
                    Darken(canvas, new Rectangle());
                    DisplaySimpleText(canvas, guidePhrase, new Point(x, y), width);
                }
            }
        }
        float DpToPx(double value)
        {
            return (float)(value * dpi);
        }
        float GetAbsoluteX(View view)
        {
            var parent = view.ParentView;
            double x = view.X;
            while (parent != null)
            {
                x += parent.X;
                parent = parent.ParentView;
            }
            return (float)x;
        }
        float GetAbsoluteY(View view)
        {
            var parent = view.ParentView;
            double y = view.Y;
            while (parent != null)
            {
                y += parent.Y;
                parent = parent.ParentView;
            }
            return (float)y;
        }
        void Darken(SKCanvas canvas, Rectangle rect)
        {
            SKPaint paint = new SKPaint
            {
                Color = SKColor.Parse("#0f3142").WithAlpha(200),
                Style = SKPaintStyle.Fill,
            };
            canvas.DrawRect(0, 0, (float)rect.X, DpToPx(App.ScreenHeight), paint); // left
            canvas.DrawRect((float)(rect.X + rect.Width), 0,
              (float)(DpToPx(App.ScreenWidth) - (rect.X + rect.Width)), DpToPx(App.ScreenHeight), paint); // right
            canvas.DrawRect((float)rect.X, 0, (float)rect.Width, (float)rect.Y, paint); // center top
            canvas.DrawRect((float)rect.X, (float)(rect.Y + rect.Height),
                (float)rect.Width, (float)(DpToPx(App.ScreenHeight) - rect.Y - rect.Height), paint); // center bottom
        }
        void DrawLightRect(SKCanvas canvas, Rectangle rect)
        {
            SKPaint paintStroke = new SKPaint
            {
                Color = SKColors.White,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 2,
            };
            canvas.DrawRoundRect((float)rect.X, (float)rect.Y, (float)rect.Width, (float)rect.Height,
                DpToPx(3), DpToPx(3), paintStroke);
        }

        void DisplaySimpleText(SKCanvas canvas, string text, Point point, float width)
        {
            SKPaint textPaint = new SKPaint
            {
                Color = SKColors.White,
                TextSize = DpToPx(20),
                IsLinearText = true,

            };
            SKPaint strokePaint = new SKPaint
            {
                Color = SKColor.Parse("#33a5de"),
                Style = SKPaintStyle.Fill,
            };
            float tlX = 0, tlY = 0, brX = 0, brY = 0; // top left X, top left Y, bottom right X, bottom right Y
            float radius = DpToPx(5);
            float textMargin = DpToPx(10);

            tlX = (float)point.X - width / 2;
            brX = (float)point.X + width / 2;

            tlY = (float)point.Y;
            brY = (float)point.Y;

            SKRect rect = new SKRect(tlX, tlY, brX, brY);
            var (areaUpdated, lines, lineHeight) = MeasureMultilineText(text, rect, textPaint);
            areaUpdated.Top -= textMargin;
            areaUpdated.Bottom += textMargin;

            SKRoundRect roundRect = new SKRoundRect(areaUpdated, radius);

            canvas.DrawRoundRect(roundRect, strokePaint);
            strokePaint.Style = SKPaintStyle.Stroke;
            strokePaint.Color = SKColors.White;
            canvas.DrawRoundRect(roundRect, strokePaint);

            DrawMultilineText(canvas, text, rect, textPaint);
        }

        public class Line
        {
            public string Value { get; set; }
            public float Width { get; set; }
        }

        private (SKRect, Line[], float) MeasureMultilineText(string text, SKRect area, SKPaint paint, 
            TextAlignment verticalTextAlignment = TextAlignment.Center)
        {
            float marginHorizontal = DpToPx(5);
            float lineHeight = paint.FontMetrics.Descent - paint.FontMetrics.Ascent;
            var lines = SplitLines(text, paint, area.Width - 2 * marginHorizontal);
            var height = lines.Count() * lineHeight;

            var areaUpdated = new SKRect(area.Left, area.MidY - height / 2, area.Right,
                area.MidY + height / 2 + paint.FontMetrics.Descent);

            if (verticalTextAlignment == TextAlignment.Center)
            {
                areaUpdated.Top = area.MidY - height / 2;
                areaUpdated.Bottom = area.MidY + height / 2;
                areaUpdated.Bottom += paint.FontMetrics.Descent;
            }
            if (verticalTextAlignment == TextAlignment.Start)
            {
                areaUpdated.Top = area.Top;
                areaUpdated.Bottom = area.Top + height;
                areaUpdated.Bottom += paint.FontMetrics.Descent;
            }
            if (verticalTextAlignment == TextAlignment.End)
            {
                areaUpdated.Top = area.Bottom - height - paint.FontMetrics.Descent;
                areaUpdated.Bottom = area.Bottom;
            }
            return (areaUpdated, lines, lineHeight);
        }
        private void DrawMultilineText(SKCanvas canvas, string text, SKRect area, SKPaint paint,
            TextAlignment verticalTextAlignment = TextAlignment.Center)
        {
            var (areaUpdated, lines, lineHeight) = MeasureMultilineText(text, area, paint, verticalTextAlignment);

            var y = areaUpdated.Top;
            foreach (var line in lines)
            {
                y += lineHeight;
                var x = areaUpdated.MidX - line.Width / 2;
                canvas.DrawText(line.Value, x, y, paint);
            }
        }

        private Line[] SplitLines(string text, SKPaint paint, float maxWidth)
        {
            var spaceWidth = paint.MeasureText(" ");
            var lines = text.Split('\n');

            return lines.SelectMany((line) =>
            {
                var result = new List<Line>();

                var words = line.Split(new[] { " " }, StringSplitOptions.None);

                var lineResult = new StringBuilder();
                float width = 0;
                foreach (var word in words)
                {
                    var wordWidth = paint.MeasureText(word);
                    var wordWithSpaceWidth = wordWidth + spaceWidth;
                    var wordWithSpace = word + " ";

                    if (width + wordWidth > maxWidth)
                    {
                        result.Add(new Line() { Value = lineResult.ToString(), Width = width - spaceWidth });
                        lineResult = new StringBuilder(wordWithSpace);
                        width = wordWithSpaceWidth;
                    }
                    else
                    {
                        lineResult.Append(wordWithSpace);
                        width += wordWithSpaceWidth;
                    }
                }

                result.Add(new Line() { Value = lineResult.ToString(), Width = width - spaceWidth });

                return result.ToArray();
            }).ToArray();
        }


        void DisplayText(SKCanvas canvas, string text, Rectangle whiteRect)
        {
            SKPaint textPaint = new SKPaint
            {
                Color = SKColors.White,
                TextSize = DpToPx(20),
                IsLinearText = true
            };
            SKPaint strokePaint = new SKPaint
            {
                Color = SKColor.Parse("#33a5de"),
                Style = SKPaintStyle.Fill,
            };
            float tlX = 0, tlY = 0, brX = 0, brY = 0; // top left X, top left Y, bottom right X, bottom right Y
            float textWidth = textPaint.MeasureText(text);
            float radius = DpToPx(5);
            float mgn = DpToPx(20);
            float marginEdge = DpToPx(15);
            float textMargin = DpToPx(5);
            float textWidening = textMargin;

            tlX = (float)(whiteRect.X + whiteRect.Width / 2 - textWidth / 2 - textWidening);
            brX = (float)(whiteRect.X + whiteRect.Width / 2 + textWidth / 2 + textWidening);

            /*
            tlX = (float)(whiteRect.X - textWidening);
            brX = (float)(whiteRect.X + textWidth + textWidening);
            float widthDiff = (float)(whiteRect.Width - textWidth);

            tlX = tlX + widthDiff / 2;
            brX = brX + widthDiff / 2;*/

            TextAlignment textAlignment;

            if (whiteRect.Y < DpToPx(App.ScreenHeight / 2)) // upper half
            {
                tlY = (float)(whiteRect.Y + whiteRect.Height + mgn - textWidening);
                brY = (float)(whiteRect.Y + whiteRect.Height + mgn + textPaint.TextSize + textWidening);
                textAlignment = TextAlignment.Start;
            }
            else  // lower half
            {
                tlY = (float)(whiteRect.Y - mgn - textPaint.TextSize - textWidening);
                brY = (float)(whiteRect.Y - mgn + textWidening);
                textAlignment = TextAlignment.End;
            }

            float leftScreenEdge = marginEdge;
            float rightScreenEdge = DpToPx(App.ScreenWidth) - marginEdge;
            if (brX > rightScreenEdge)
            {
                tlX = rightScreenEdge - (brX - tlX);
                brX = rightScreenEdge;
            }

            if (tlX < leftScreenEdge)
            {
                brX = brX - tlX + leftScreenEdge;
                tlX = leftScreenEdge;
            }
            if (brX > rightScreenEdge)
            {
                brX = rightScreenEdge;
            }

            /*SKRect rect = new SKRect(tlX, tlY, brX, brY);
            SKRoundRect roundRect = new SKRoundRect(rect, radius);

            canvas.DrawRoundRect(roundRect, strokePaint);
            strokePaint.Style = SKPaintStyle.Stroke;
            strokePaint.Color = SKColors.White;
            canvas.DrawRoundRect(roundRect, strokePaint);

            float textHeight = textPaint.FontMetrics.Descent - textPaint.FontMetrics.Ascent;
            float textOffset = (textHeight / 2) - textPaint.FontMetrics.Descent;
            canvas.DrawText(text, rect.MidX - textWidth / 2, rect.MidY + textOffset, textPaint);*/

            SKRect rect = new SKRect(tlX, tlY, brX, brY);
            var (areaUpdated, lines, lineHeight) = MeasureMultilineText(text, rect, textPaint, textAlignment);
            areaUpdated.Top -= textMargin;
            areaUpdated.Bottom += textMargin;

            SKRoundRect roundRect = new SKRoundRect(areaUpdated, radius);

            canvas.DrawRoundRect(roundRect, strokePaint);
            strokePaint.Style = SKPaintStyle.Stroke;
            strokePaint.Color = SKColors.White;
            canvas.DrawRoundRect(roundRect, strokePaint);

            DrawMultilineText(canvas, text, rect, textPaint, textAlignment);
        }

    }
}
