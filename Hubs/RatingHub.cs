using CEPAggregator.Classes;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CEPAggregator.Hubs
{
    public class RatingHub : Hub
    {
        public async Task SendRating(string message)
        {
            var rating = OnnxModelWrapper.Predict(message);
            await Clients.Caller.SendAsync("GetRating", rating);
        }
    }
}
