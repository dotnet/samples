// <Snippet1>
using Microsoft.ML.Runtime.Api;
// </Snippet1>

namespace TaxiFarePrediction
{
    // <Snippet2>
    public class TaxiTrip
    {
        [Column(ordinal: "0")]
        public string vendor_id;
        [Column(ordinal: "1")]
        public string rate_code;
        [Column(ordinal: "2")]
        public float passenger_count;
        [Column(ordinal: "3")]
        public float trip_time_in_secs;
        [Column(ordinal: "4")]
        public float trip_distance;
        [Column(ordinal: "5")]
        public string payment_type;
        [Column(ordinal: "6")]
        public float fare_amount;
    }
    // </Snippet2>

    // <Snippet3>
    public class TaxiTripFarePrediction
    {
        [ColumnName("Score")]
        public float fare_amount;
    }
    // </Snippet3>
}
