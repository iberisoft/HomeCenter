namespace HomeCenter.Virtual
{
    public class Switch
    {
        public string Status { get; private set; }

        public void SetStatus(string status) => Status = status;

        public override string ToString() => $"Last status: {Status}";
    }
}
