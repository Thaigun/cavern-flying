using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CavernWars
{
    [RequireComponent(typeof(MeshRenderer))]
    public class MaterialChanger : MonoBehaviour
    {
        [SerializeField]
        private List<Material> _materials;

        private MeshRenderer _meshRenderer;

        // Use this for initialization
        public void UpdateColor(string name)
        {
            if (PartyManager.Instance == null)
            {
                return;
            }

            _meshRenderer = GetComponent<MeshRenderer>();
            var orderedPlayers = PartyManager.Instance.Players.OrderBy(plr => plr.Name);
            int thisPlayerIdx = 0;
            foreach (var player in orderedPlayers)
            {
                if (player.Name == name)
                {
                    break;
                }
                thisPlayerIdx++;
            }
            _meshRenderer.material = this._materials[thisPlayerIdx];
        }
    }
}