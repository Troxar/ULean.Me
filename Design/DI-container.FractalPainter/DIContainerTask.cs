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
            container.Bind<AppSettings>().ToMethod(context => context.Kernel.Get<SettingsManager>().Load()).InSingletonScope();
            container.Bind<IImageHolder, PictureBoxImageHolder>().To<PictureBoxImageHolder>().InSingletonScope();
            container.Bind<ImageSettings>().ToMethod(context => context.Kernel.Get<AppSettings>().ImageSettings).InSingletonScope();
            container.Bind<Palette>().ToSelf().InSingletonScope();
            container.Bind<IDragonPainterFactory>().ToFactory();
            container.Bind<IKochPainterFactory>().ToFactory();
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
        private readonly IKochPainterFactory _painterFactory;

        public MenuCategory Category => MenuCategory.Fractals;
        public string Name => "Кривая Коха";
        public string Description => "Кривая Коха";

        public KochFractalAction(IKochPainterFactory painterFactory)
        {
            _painterFactory = painterFactory;
        }

        public void Perform()
        {
            var painter = _painterFactory.CreateKochPainter();
            painter.Paint();
        }
    }

    public class DragonPainter
    {
        private readonly IImageHolder imageHolder;
        private readonly Palette palette;
        private readonly DragonSettings settings;
        private readonly float size;
        private Size imageSize;

        public DragonPainter(IImageHolder imageHolder, Palette palette, DragonSettings settings)
        {
            this.imageHolder = imageHolder;
            this.palette = palette;
            this.settings = settings;
            imageSize = imageHolder.GetImageSize();
            size = Math.Min(imageSize.Width, imageSize.Height) / 2.1f;
        }

        public void Paint()
        {
            using (var graphics = imageHolder.StartDrawing())
            using (var backgroundBrush = new SolidBrush(palette.BackgroundColor))
            using (var penBrush = new SolidBrush(palette.PrimaryColor))
            {
                graphics.FillRectangle(backgroundBrush, 0, 0, imageSize.Width, imageSize.Height);
                var r = new Random();
                var cosa = (float)Math.Cos(settings.Angle1);
                var sina = (float)Math.Sin(settings.Angle1);
                var cosb = (float)Math.Cos(settings.Angle2);
                var sinb = (float)Math.Sin(settings.Angle2);
                var shiftX = settings.ShiftX * size * 0.8f;
                var shiftY = settings.ShiftY * size * 0.8f;
                var scale = settings.Scale;
                var p = new PointF(0, 0);
                foreach (var i in Enumerable.Range(0, settings.IterationsCount))
                {
                    graphics.FillRectangle(penBrush, imageSize.Width / 3f + p.X, imageSize.Height / 2f + p.Y, 1, 1);
                    if (r.Next(0, 2) == 0)
                        p = new PointF(scale * (p.X * cosa - p.Y * sina), scale * (p.X * sina + p.Y * cosa));
                    else
                        p = new PointF(scale * (p.X * cosb - p.Y * sinb) + shiftX, scale * (p.X * sinb + p.Y * cosb) + shiftY);
                    if (i % 100 == 0) imageHolder.UpdateUi();
                }
            }
            imageHolder.UpdateUi();
        }
    }

    public interface IDragonPainterFactory
    {
        DragonPainter CreateDragonPainter(DragonSettings settings);
    }

    public interface IKochPainterFactory
    {
        KochPainter CreateKochPainter();
    }
}
