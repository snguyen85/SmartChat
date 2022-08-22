using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SmartChat.Shared.Models;
using System.IO;

namespace SmartChat.Data.Migrations
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<UserConversation> UserConversations { get; set; } 
        public DbSet<DirectMessage> DirectMessages { get; set; }
        public DbSet<RoomMessage> RoomMessages { get; set; }    
        public DbSet<RoomMembership> RoomMembers { get; set; }
        public DbSet<ApplicationUser> SmartUsers { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}