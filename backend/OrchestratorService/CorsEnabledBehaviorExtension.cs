using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace OrchestratorService
{
    internal class CorsEnabledBehaviorExtension : BehaviorExtensionElement
    {
        public override Type BehaviorType => typeof(CorsEnabledBehavior);

        protected override object CreateBehavior()
        {
            return new CorsEnabledBehavior();
        }
    }
}