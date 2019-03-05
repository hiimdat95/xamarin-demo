using System.Collections.Generic;
using System.Linq;
using TravellerApp.Models;

namespace TravellerApp.Mapper
{
    public static partial class Mapper
    {
        public static List<ListInfo> toModel(List<Places> lists)
        {
            if (lists == null) return null;
            return lists.Select(t => ToDataTransferObject(t)).ToList();
        }

        public static ListInfo ToDataTransferObject(Places viewModel)
        {
            if (viewModel == null)
                return null;
            return new ListInfo()
            {
                id = viewModel.id,
                image = viewModel.image,
                partner_latitude = viewModel.latitude,
                partner_longitude = viewModel.longitude,
                name = viewModel.name,
                distance = viewModel.distance,
                distance_display = viewModel.distance_display
            };
        }
    }
}
