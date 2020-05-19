using JHchoi.Module.Detection.CV;

namespace JHchoi.Module.Detection
{
    public static class DetectionGraphFactory
    {
        public static DetectionGraph Build(ContentType contentType)
        {
            IGraphBuilder<DetectionGraph> builder = null;
            switch (contentType)
            {
                case ContentType.NerfGun:
                    builder = new NerfGunBuilder();
                    break;

                case ContentType.Laser:
                    builder = new LaserBuilder();
                    break;

                case ContentType.Blade:
                    builder = new BladeBuilder();
                    break;

                case ContentType.FishCatch:
                    builder = new FishCatchBuilder();
                    break;

                case ContentType.Waterplay:
                    builder = new WaterplayBuilder();
                    break;
            }

            return builder.Build();
        }
    }
}