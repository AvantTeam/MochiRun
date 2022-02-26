public class GainShieldUpdater : ObstacleUpdater {
    public override void OnHit() {
        pcon.AddItem(new ShieldItem());
    }
}
