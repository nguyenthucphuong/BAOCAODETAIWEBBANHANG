namespace SaleApi.Models.Accounts
{
    public class TokenBlacklist
    {
        private static Dictionary<string, DateTime> _tokens = new Dictionary<string, DateTime>();
        //Cho phép truy cập vào từ điển _tokens từ bên ngoài lớp, nhưng chỉ cho đọc Tokens và không cho phép thay đổi nội dung
        public static IReadOnlyDictionary<string, DateTime> Tokens => _tokens;
        public static void AddToken(string token, DateTime expires)
        {
            _tokens[token] = expires;
            Console.WriteLine($"Đã thêm token: {token}");
            foreach (var toKen in _tokens)
            {
                Console.WriteLine($"Token: {toKen.Key}, Expires: {toKen.Value}");
            }
        }
        public static void RemoveExpiredTokens()
        {
            var expiredTokens = _tokens.Where(t => t.Value < DateTime.UtcNow).Select(t => t.Key).ToList();
            foreach (var token in expiredTokens)
            {
                _tokens.Remove(token);
            }
        }
    }
}
