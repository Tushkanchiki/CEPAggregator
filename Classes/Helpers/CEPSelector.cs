using CEPAggregator.Data;
using CEPAggregator.Enums;
using CEPAggregator.Models;
using GeoCoordinatePortable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CEPAggregator.Classes.Helpers
{
    public class CEPSelector
    { 
        public struct SelectionParams
        {
            public bool useLocation;
            public bool useRating;
            public bool useCurrency;
            public int selectCnt;
        }

        public class EvaluatedCEP
        {
            public CEP Cep { get; set; }
            public double? Distance { get; set; }
            public double? Rating { get; set; }
            public double? Currency { get; set; }
        }

        private static double CalculateDistance(double x1, double y1, double x2, double y2)
        {
            var coord1 = new GeoCoordinate(x1, y1);
            var coord2 = new GeoCoordinate(x2, y2);
            return coord1.GetDistanceTo(coord2) / 1000;
        }

        public const double BaseDistanceStep = 7;
        public const double AverageRating = 2.5;
        public static List<double> DistanceMilestones = new List<double> { 1, 2, 3, 4, 100 };
        public static List<double> RatingMilestones = new List<double> { 4.5, 3.5, 2, 0 };

        private readonly double _userX;
        private readonly double _userY;
        private readonly CurrencyType _currencyType;
        private readonly ApplicationDbContext _dbContext;

        public CEPSelector(double userX, double userY, CurrencyType currencyType, ApplicationDbContext dbContext)
        {
            _userX = userX;
            _userY = userY;
            _currencyType = currencyType;
            _dbContext = dbContext;
        }

        private void EvaluateWithDistance(List<EvaluatedCEP> ceps)
        {
            foreach (var cep in ceps)
            {
                if (cep.Distance == null)
                {
                    cep.Distance = CalculateDistance(_userX, _userY, cep.Cep.X, cep.Cep.Y);
                }
            }
        }

        private void EvaluateWithRating(List<EvaluatedCEP> ceps)
        {
            foreach (var cep in ceps)
            {
                if (cep.Rating == null)
                {
                    var ratings = _dbContext.Comments.Where(c => c.CEP == cep.Cep).Select(c => c.Rating);
                    if (ratings.Any())
                    {
                        cep.Rating = ratings.Average();
                    }
                    else
                    {
                        cep.Rating = AverageRating;
                    }
                }
            }
        }

        private void EvaluateWithCurrency(List<EvaluatedCEP> ceps)
        {
            foreach (var cep in ceps)
            {
                if (cep.Currency == null)
                {

                }
            }
        }

        private List<EvaluatedCEP> SelectCEPs(List<EvaluatedCEP> ceps, SelectionParams selParams)
        {
            if (selParams.useLocation)
            {
                EvaluateWithDistance(ceps);
                ceps = new List<EvaluatedCEP>(ceps.OrderBy(cep => cep.Distance));
                if (!selParams.useRating && !selParams.useCurrency)
                {
                    return ceps.GetRange(0, selParams.selectCnt);
                }
                int nextIndex = 0;
                var res = new List<EvaluatedCEP>();
                SelectionParams newParams = selParams;
                newParams.useLocation = false;
                var milestoneMult = Math.Max(BaseDistanceStep, ceps[0].Distance ?? 0);
                int leftToSelect = selParams.selectCnt;
                for (int i = 0; i < DistanceMilestones.Count; i++)
                {
                    while (nextIndex < ceps.Count && ceps[nextIndex].Distance <= DistanceMilestones[i] * milestoneMult)
                    {
                        nextIndex++;
                    }
                    var sublist = ceps.GetRange(0, nextIndex);
                    sublist = new List<EvaluatedCEP>(sublist.Where(i => !res.Contains(i)));
                    newParams.selectCnt = Math.Min(sublist.Count, leftToSelect / (DistanceMilestones.Count - i));
                    var selected = SelectCEPs(sublist, newParams);
                    leftToSelect -= selected.Count;
                    res = res.Concat(selected).ToList();
                }
                return res;
            }
            else if (selParams.useRating)
            {
                EvaluateWithRating(ceps);
                ceps = new List<EvaluatedCEP>(ceps.OrderBy(cep => -cep.Rating));
                if (!selParams.useCurrency)
                {
                    return ceps.GetRange(0, selParams.selectCnt);
                }
                int nextIndex = 0;
                var res = new List<EvaluatedCEP>();
                SelectionParams newParams = selParams;
                newParams.useRating = false;
                int leftToSelect = selParams.selectCnt;
                for (int i = 0; i < RatingMilestones.Count; i++)
                {
                    while (nextIndex < ceps.Count && ceps[nextIndex].Rating >= RatingMilestones[i])
                    {
                        nextIndex++;
                    }
                    var sublist = ceps.GetRange(0, nextIndex);
                    sublist = new List<EvaluatedCEP>(sublist.Where(i => !res.Contains(i)));
                    newParams.selectCnt = Math.Min(sublist.Count, leftToSelect / (RatingMilestones.Count - i));
                    var selected = SelectCEPs(sublist, newParams);
                    leftToSelect -= selected.Count;
                    res = res.Concat(selected).ToList();
                }
                return res;
            }
            else if (selParams.useCurrency)
            {
                EvaluateWithCurrency(ceps);
                ceps = new List<EvaluatedCEP>(ceps.OrderBy(cep => cep.Currency));
                return ceps.GetRange(0, selParams.selectCnt);
            }
            else
            {
                return ceps.GetRange(0, selParams.selectCnt);
            }
        }

        public List<EvaluatedCEP> SelectCEPs(List<CEP> ceps, SelectionParams selParams)
        {
            var evaluatedCeps = new List<EvaluatedCEP>();
            foreach (var cur_cep in ceps)
            {
                evaluatedCeps.Add(new EvaluatedCEP{Cep = cur_cep});
            }
            return SelectCEPs(evaluatedCeps, selParams);
        }
    }
}
