using Cinema.Domain.DomainModels;
using Cinema.Domain.DTO;
using Cinema.Repository.Interface;
using Cinema.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinema.Service.Implementation
{
    public class ShoppingCartService : IShoppingCartService
    {

        private readonly IRepository<ShoppingCart> _shoppingCartRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<TicketInOrder> _ticketInOrderRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRepository<EmailMessage> _mailRepository;

        public ShoppingCartService(IRepository<ShoppingCart> shoppingCartRepository, IRepository<Order> orderRepository, IRepository<TicketInOrder> ticketInOrderRepository, IUserRepository userRepository, IRepository<EmailMessage> mailRepository)
        {
            _shoppingCartRepository = shoppingCartRepository;
            _orderRepository = orderRepository;
            _ticketInOrderRepository = ticketInOrderRepository;
            _userRepository = userRepository;
            _mailRepository = mailRepository;
        }

        public bool deleteTicketFromShoppingCart(string userId, Guid ticketId)
        {
            if(!string.IsNullOrEmpty(userId) && ticketId != null)
            {
                var loggedInUser = this._userRepository.Get(userId);
                var userShoppingCart = loggedInUser.UserCart;

                var ticketToRemove = userShoppingCart.TicketInShoppingCarts.Where(z => z.TicketId.Equals(ticketId)).FirstOrDefault();
                userShoppingCart.TicketInShoppingCarts.Remove(ticketToRemove);

                this._shoppingCartRepository.Update(userShoppingCart);
                return true;
            }
            return false;
        }

        public ShoppingCartDto getShoppingCartInfo(string userId)
        {
            var loggedInUser = this._userRepository.Get(userId);
            var userShoppingCart = loggedInUser.UserCart;
            var allTickets = userShoppingCart.TicketInShoppingCarts.ToList();

            var ticketPrice = userShoppingCart.TicketInShoppingCarts.Select(z => new
            {
                TicketPrice = z.Ticket.TicketPrice,
                Quantity = z.Quantity
            }).ToList();

            double totalPrice = 0;
            foreach (var item in ticketPrice)
            {
                totalPrice += item.TicketPrice * item.Quantity;
            }

            ShoppingCartDto shoppingCartDtoItem = new ShoppingCartDto
            {
                TicketInShoppingCarts = allTickets,
                TotalPrice = totalPrice
            };

            return shoppingCartDtoItem;
        }

        public bool order(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                var loggedInUser = this._userRepository.Get(userId);
                var userShoppingCart = loggedInUser.UserCart;

                EmailMessage mail = new EmailMessage();
                mail.MailTo = loggedInUser.Email;
                mail.Subject = "Sucessfully created order!";
                mail.Status = false;

                Order orderItem = new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    User = loggedInUser,

                };

                this._orderRepository.Insert(orderItem);

                List<TicketInOrder> ticketInOrders = new List<TicketInOrder>();
                ticketInOrders = userShoppingCart.TicketInShoppingCarts
                    .Select(z => new TicketInOrder
                    {
                        OrderId = orderItem.Id,
                        TicketId = z.TicketId,
                        SelectedTicket = z.Ticket,
                        UserOrder = orderItem,
                        Quantity=z.Quantity //added
                    }).ToList();

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Your order is completed. The order contains: ");
                var total = 0.0;
                for (int i = 1; i <= ticketInOrders.Count(); i++)
                {
                    var item = ticketInOrders[i - 1];
                    total += item.Quantity * item.SelectedTicket.TicketPrice;
                    sb.AppendLine("- " + item.Quantity + " tickets for movie " + item.SelectedTicket.MovieName + " on " + item.SelectedTicket.ProjectionDateTime + " in " + item.SelectedTicket.CinemaHall + ", each with price of $" + item.SelectedTicket.TicketPrice);

                }

                sb.AppendLine("Total price for your order: $" + total.ToString());

                mail.Content = sb.ToString();

                foreach (var item in ticketInOrders)
                {
                    this._ticketInOrderRepository.Insert(item);
                }

                loggedInUser.UserCart.TicketInShoppingCarts.Clear();
                this._mailRepository.Insert(mail);
                this._userRepository.Update(loggedInUser);
                return true;
            }

            return false;

        }
    }
}
