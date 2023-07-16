using System;
using System.Drawing;
using System.Linq;
using FractalPainting.App.Fractals;
using FractalPainting.Infrastructure.Common;
using FractalPainting.Infrastructure.UiActions;
using Ninject;
using Ninject.Extensions.Conventions;
using Ninject.Extensions.Factory;

namespace FractalPainting.App
{
    public static class DIContainerTask
    {
        public static MainForm CreateMainForm()
        {
            var container = ConfigureContainer();
            return container.Get<MainForm>();
        }

        public static StandardKernel ConfigureContainer()
        {
            var container = new StandardKernel();
            container.Bind<MainForm>().ToSelf().InSingletonScope();
            container.Bind(config => config.FromThisAssembly()
                .SelectAllClasses()
                .InheritedFrom<IUiAction>()
                .BindAllInterfaces());
            container.Bind<AppSettings>().ToMethod(context => context.Kernel.Get<SettingsManager>().Load())
                .InSingletonScope();
            container.Bind<IImageHolder, PictureBoxImageHolder>().To<PictureBoxImageHolder>()
                .InSingletonScope();
            container.Bind<ImageSettings>().ToMethod(context => context.Kernel.Get<AppSettings>().ImageSettings)
                .InSingletonScope();
            container.Bind<Palette>().ToSelf().InSingletonScope();
            container.Bind<IDragonPainterFactory>().ToFactory();
            container.Bind<IObjectSerializer>().To<XmlObjectSerializer>()
                .WhenInjectedInto<SettingsManager>().InSingletonScope();
            container.Bind<IBlobStorage>().To<FileBlobStorage>()
                .WhenInjectedInto<SettingsManager>().InSingletonScope();
            container.Bind<SettingsManager>().ToSelf().InSingletonScope();

            return container;
        }
    }

    public class DragonFractalAction : IUiAction
    {
        private readonly IDragonPainterFactory _painterFactory;

        public MenuCategory Category => MenuCategory.Fractals;
        public string Name => "Дракон";
        public string Description => "Дракон Хартера-Хейтуэя";

        public DragonFractalAction(IDragonPainterFactory painterFactory)
        {
            _painterFactory = painterFactory;
        }

        public void Perform()
        {
            var dragonSettings = CreateRandomSettings();
            SettingsForm.For(dragonSettings).ShowDialog();
            var painter = _painterFactory.CreateDragonPainter(dragonSettings);
            painter.Paint();
        }

        private static DragonSettings CreateRandomSettings()
        {
            return new DragonSettingsGenerator(new Random()).Generate();
        }
    }

    public class KochFractalAction : IUiAction
    {
        private readonly Lazy<KochPainter> _painter;

        public MenuCategory Category => MenuCategory.Fractals;
        public string Name => "Кривая Коха";
        public string Description => "Кривая Коха";

        public KochFractalAction(Lazy<KochPainter> painter)
        {
            _painter = painter;
        }

        public void Perform()
        {
            _painter.Value.Paint();
        }
    }

    public class DragonPainter
    {
        private readonly IImageHolder imageHolder;
        private readonly Palette palette;
        private readonly DragonSettings settings;
        
        public DragonPainter(IImageHolder imageHolder, Palette palette, DragonSettings settings)
        {
            this.imageHolder = imageHolder;
            this.palette = palette;
            this.settings = settings;
        }

        public void Paint()
        {
            var imageSize = imageHolder.GetImageSize();
            var size = Math.Min(imageSize.Width, imageSize.Height) / 2.1f;

            using (var graphics = imageHolder.StartDrawing())
            using (var backgroundBrush = new SolidBrush(palette.BackgroundColor))
            using (var penBrush = new SolidBrush(palette.PrimaryColor))
            {
                graphics.FillRectangle(backgroundBrush, 0, 0, imageSize.Width, imageSize.Height);
                var parameters = new DragonPaintParameters(settings, size);
                PaintImage(graphics, imageSize, penBrush, parameters);
            }
            imageHolder.UpdateUi();
        }

        private void PaintImage(Graphics graphics, Size imageSize,
            SolidBrush penBrush, DragonPaintParameters ddp)
        {
            var p = new PointF(0, 0);
            var r = new Random();

            foreach (var i in Enumerable.Range(0, settings.IterationsCount))
            {
                graphics.FillRectangle(penBrush, imageSize.Width / 3f + p.X, imageSize.Height / 2f + p.Y, 1, 1);
                if (r.Next(0, 2) == 0)
                    p = new PointF(ddp.Scale * (p.X * ddp.CosA - p.Y * ddp.SinA),
                        ddp.Scale * (p.X * ddp.SinA + p.Y * ddp.CosA));
                else
                    p = new PointF(ddp.Scale * (p.X * ddp.CosB - p.Y * ddp.SinB) + ddp.ShiftX,
                        ddp.Scale * (p.X * ddp.SinB + p.Y * ddp.CosB) + ddp.ShiftY);
                if (i % 100 == 0) imageHolder.UpdateUi();
            }
        }

        private class DragonPaintParameters
        {
            public readonly float CosA;
            public readonly float CosB;
            public readonly float SinA;
            public readonly float SinB;
            public readonly float ShiftX;
            public readonly float ShiftY;
            public readonly float Scale;

            public DragonPaintParameters(DragonSettings settings, float size)
            {
                CosA = (float)Math.Cos(settings.Angle1);
                CosB = (float)Math.Cos(settings.Angle2);
                SinA = (float)Math.Sin(settings.Angle1);
                SinB = (float)Math.Sin(settings.Angle2);
                ShiftX = settings.ShiftX * size * 0.8f;
                ShiftY = settings.ShiftY * size * 0.8f;
                Scale = settings.Scale;
            }
        }
    }

    public interface IDragonPainterFactory
    {
        DragonPainter CreateDragonPainter(DragonSettings settings);
    }
}
