using Other;
using UI;
using UnityEngine;
namespace Managers
{
    public class UIManager : MonoSingleton<UIManager>
    {
        [SerializeField] private GameUI gameUI;
        public GameUI GameUI => gameUI;

        [SerializeField] private InformationUI informationUI;
        public InformationUI InformationUI => informationUI;
    }
}
