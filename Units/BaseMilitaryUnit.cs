namespace RTS_LEARN.Units
{
    public class BaseMilitaryUnit : AbstractUnit, ITransportable
    {
        public int TransportCapacityUsage => unitSO.TransportConfig.GetTransportCapacityUsage();

        public void LoadInto(ITransporter transporter)
        {
            MoveTo(transporter.Transform);
            transporter.Load(this);
        }
    }
}