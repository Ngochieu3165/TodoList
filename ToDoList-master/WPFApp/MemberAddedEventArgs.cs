using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFApp
{
    public class MemberAddedEventArgs : EventArgs
    {
            public User NewMember { get; }

            public MemberAddedEventArgs(User newMember)
            {
                NewMember = newMember;
            }
    }
}
