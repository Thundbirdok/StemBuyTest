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
        private SpriteRenderer healthBar;
        
        [SerializeField]
        private Color ownedColor;
        
        [SerializeField]
        private Color unownedColor;

        public void Paint(bool isOwner)
        {
            var color = isOwner ? ownedColor : unownedColor;
            
            body.color = color;
            healthBar.color = color;
        }
    }
}
