namespace BuzzBid.Models
{
	public class LoadCommentsModel
	{

        public String ItemId { get; set; } 

		public String ItemName { get; set; }

		public String CommentsText { get; set; }

        public String RatedBy {get; set;}

        public int Stars {get; set;}

        public String RatedTime {get; set;}



	}
}
