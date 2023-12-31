﻿

namespace RedCrossChat.Entities
{
    public class HandOverRequest  : BaseEntity
    {

        public string Title { get; set; }

        public bool HasBeenReceived { get; set; }

        public bool HasResponse { get; set; } = false;

        public bool isActive { get; set; }=false;

        public DateTime? ResolvedAt { get; set; }

        public AppUser? AppUser { get; set; }

        public Guid? AppUserId { get; set; }

        public Conversation Conversation { get; set; }

        public Guid ConversationId { get; set; }

        public ChatMessage? LastChatMessage { get; set; }

        public Guid? LastChatMessageId { get; set; }

    }
}
