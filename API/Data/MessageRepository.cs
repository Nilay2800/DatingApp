using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public MessageRepository(DataContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public void AddMessage(Message message)
        {
            _context.Message.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Message.Remove(message);
        }

        public async Task<IEnumerable<MessageDto>> GetMeaasgeThread(string currentUsername, string recipientUsername)
        {
            var messages = await _context.Message
                            .Where(m => m.Recipient.UserName == currentUsername
                             && m.RecipientDeleted==false
                             && m.Sender.UserName==recipientUsername
                             || m.Recipient.UserName==recipientUsername
                             && m.Sender.UserName==currentUsername && m.SenderDeleted==false 
                        )
                        .OrderBy(m=>m.MessageSent)
                        .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
                        .ToListAsync();
            var unreadMessages = messages.Where(m => m.DateRead == null
                                      && m.RecipientUsername == currentUsername).ToList();
            if (unreadMessages.Any())
            {
                foreach(var message in unreadMessages)
                {
                    message.DateRead = DateTime.Now;
                }

                await _context.SaveChangesAsync();
            }

            return messages;
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Message
                        .Include(u=>u.Sender)
                        .Include(u=>u.Recipient)
                        .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<pagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _context.Message.OrderByDescending(m => m.MessageSent).AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u => u.Recipient.UserName == messageParams.Username && 
                            u.RecipientDeleted==false),
                "Outbox" => query.Where(u => u.Sender.UserName == messageParams.Username &&
                            u.SenderDeleted==false),
                _ => query.Where(u => u.RecipientUsername == messageParams.Username && u.RecipientDeleted==false 
                            && u.DateRead == null)
            };

            var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);
            return await pagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
