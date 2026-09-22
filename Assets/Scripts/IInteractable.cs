
using System.Threading.Tasks;

public interface IInteractable 
{
   Task Interact();
   bool CanInteract();
}
