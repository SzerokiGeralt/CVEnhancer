using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CVEnhancer.DTO;
using CVEnhancer.Models;

namespace CVEnhancer.Services
{
    public class AnalysisService
    {
        private readonly LocalDbService _db;
        private readonly SessionService _session;
        private readonly ExtractionService _extraction;
        private readonly MatchingService _matching;

        public AnalysisService(LocalDbService db, SessionService session, ExtractionService extraction, MatchingService matching)
        {
            _db = db;
            _session = session;
            _extraction = extraction;
            _matching = matching;
        }

        public async Task<MatchingResultDTO> AnalyzeJobOfferAsync(string jobText)
        {
            if (_session.ActiveUser == null)
                throw new InvalidOperationException("Brak zalogowanego użytkownika.");

            var user = await _db.GetUserWithAllData(_session.ActiveUser.UserId);
            if (user == null)
                throw new InvalidOperationException("Nie znaleziono użytkownika w bazie.");

            var result = await _matching.MatchAsync(user, jobText, topN: 10);

            return result;
        }
    }
}
