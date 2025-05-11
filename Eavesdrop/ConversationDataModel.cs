using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eavesdrop;

internal class ConversationDataModel
{
    internal List<Participant> Participants = [];
    internal string Condition = string.Empty;
    internal int Precedence = 0;
    internal bool Repeatable = false;
}

internal class Participant
{
    internal bool Optional = false;
    internal bool CameraTracking = true;
    internal string FollowUpDialogue = "normal"; //optionals are Normal, Silent, and <string key>
}
