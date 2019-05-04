using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logic
{
    public abstract class LogicNode : CEvent
	{
        public override void Finish()
        {
            base.Finish();
            string nextLogic = GetEventFactory().GetNextLogic();
            if (nextLogic!="")
            {
                tEvent evt = StartEvent(nextLogic);
                if (evt!=null)
                {
                    evt.DoEvent();
                }
            }
        }
	}

    class DefineLogicFactory<T> : BaseFactory      
        where T : LogicNode, new()
    {
        public override tEvent NewEvent() { return new T(); }

        public DefineLogicFactory(string nextLogicName)
        {
            mNextLogicName = nextLogicName;
        }
		
		 public override string GetNextLogic() { return mNextLogicName; }

        public string mNextLogicName;
    }

   
}
