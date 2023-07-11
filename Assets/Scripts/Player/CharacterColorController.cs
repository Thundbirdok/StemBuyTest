namespace Player
{
    using System;
    using UnityEngine;
    
    [Serializable]
    public class CharacterColorController
    {
        [SerializeField]
        private SpriteRenderer body;
        
        [SerializeField]
        private Color ownedColor;
        
        [SerializeField]
        private Color unownedColor;

        public void Paint(bool isOwner) => body.color = isOwner ? ownedColor : unownedColor;
    }
}
