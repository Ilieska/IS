using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cinema.Domain.DomainModels
{
    public class Ticket : BaseEntity
    {
        [Required]
        [Display(Name = "Movie Poster")]
        public string MoviePoster { get; set; }
        [Required]
        [Display(Name = "Movie Name")]
        public string MovieName { get; set; }
        [Required]
        [Display(Name = "Movie Director")]
        public string MovieDirector { get; set; }
        [Required]
        [Display(Name = "Movie Length")]
        public string MovieLength { get; set; }
        [Required]
        [Display(Name = "Movie Genre")]
        public string MovieGenre { get; set; }
        [Required]
        [Display(Name = "Cinema Hall")]
        public string CinemaHall { get; set; }
        [Required]
        [Display(Name = "Projection Date&Time")]
        public string ProjectionDateTime { get; set; }
        [Required]
        [Display(Name = "Ticket Price")]
        public double TicketPrice { get; set; }
        public ICollection<TicketInShoppingCart> TicketInShoppingCarts { get; set; }
        public virtual ICollection<TicketInOrder> TicketInOrders { get; set; }
    }
}
