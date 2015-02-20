using GameLibrary.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Data.Azure.Model
{
    /// <summary>
    /// Used to track pending actions to be committed to the Azure tables.
    /// </summary>
    public class AzureAction
    {
        /// <summary>
        /// The model pending an action for Azure storage. 
        /// </summary>
        public BaseModel Model {get; private set;}
        /// <summary>
        /// The database command to be performed. 
        /// </summary>
        public ActionType Action { get; private set; }

        /// <summary>
        /// Returns an action that can be passed to an Azure repository to be committed. 
        /// </summary>
        /// <param name="model">The model pending an action for Azure storage. </param>
        /// <param name="action">The database command to be performed. </param>
        public AzureAction(BaseModel model, ActionType action)
        {
            Model = model;
            Action = action;
        }

        /// <summary>
        /// Updates the pending action on the AzureAction object. Not all updates are possible and may be ignored or throw exceptions.
        /// </summary>
        /// <param name="action">The new database action to be performed.</param>
        public void UpdateAction(ActionType action)
        {
            if (Action == ActionType.Insert)
            {
                if (action == ActionType.Delete || action == ActionType.Ignore)
                    Action = ActionType.Ignore;
            }
            else if (Action == ActionType.Update)
            {
                if (action == ActionType.Delete || action == ActionType.Ignore)
                    Action = action;
            }
            else if (Action == ActionType.Ignore)
            {
                Action = action;
            }
            else if (Action == ActionType.Delete)
            {
                if (action == ActionType.Insert || action == ActionType.Update)
                    throw new Exception("This object has been deleted and can not be inserted or updated.");
                if (action == ActionType.Ignore)
                    Action = action;
            }
        }

        
    }

    /// <summary>
    /// The type of action that is waiting to be performed.
    /// </summary>
    public enum ActionType
    {
        /// <summary>
        /// Sends object as an insert command.
        /// </summary>
        Insert,
        /// <summary>
        /// Sends object as an update command.
        /// </summary>
        Update,
        /// <summary>
        /// Deletes object from storage. 
        /// </summary>
        Delete,
        /// <summary>
        /// This action will be ignored by the repository and removed. 
        /// </summary>
        Ignore
    }
}
