using BusinessObjects;
namespace WPFApp
{
    public class TeamAddedEventArgs : EventArgs
    {
        public BusinessObjects.Team NewTeam { get; }
        public TeamAddedEventArgs(BusinessObjects.Team newTeam)
        {
            NewTeam = newTeam;
        }
    }
}

