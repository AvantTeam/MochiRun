public class GainGBeltUpdater : ObstacleUpdater {
    public override void OnHit() {
        pcon.AddItem(new GravityBelt());
    }
}
