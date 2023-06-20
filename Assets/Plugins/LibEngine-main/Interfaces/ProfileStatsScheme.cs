using LibEngine.Auth;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace LibEngine.Auth
{
    public interface IUserProfileScheme
    {
        string UserName { get; }
        string Avatar { get; }
    }

    [System.Serializable]
    public class UserProfileScheme : IUserProfileScheme
    {
        public string UserName { get; set; }
        public string Avatar { get; set;  }
    }

    public interface IProfileStatsScheme
    {
        int Id { get; }
        int Coins { get; }
        int Xp { get; }
        int Mmr { get; }
    }

    [System.Serializable]
    public class ProfileStatsScheme : IProfileStatsScheme
    {
        public int Id { get; set; }
        public int Coins { get; set; }
        public int Xp { get; set; }
        public int Mmr { get; set; }
    }
}

namespace LibEngine.Client
{
    public class UserProfileMeta : IUserProfileScheme
    {
        public string UserName { get; set; }
        public string Avatar { get; }
        public string MetamaskAddress { get; }
        public bool Newsletter { get; }
        public string Uuid { get; }
        public string Email { get; }
        public List<int> Items { get; }
        public string Guild { get; }
        public string HunterCoins { get; }
        public List<WalletToken> Wallet { get; }
        public string AboutMe { get; }
        public string DiscordLink { get; }
        public string TwitterLink { get; }
        public string TelegramLink { get; }
        public DateTime DateJoined { get; }
        public string ReferralCode { get; }
        public string Country { get; }
        public string City { get; }

        //public UserProfileMeta(dynamic data)
        //{
        //    UserName = data.username;
        //    Avatar = data.avatar;
        //    MetamaskAddress = data.metamask_address;
        //    Newsletter = data.newsletter;
        //    Uuid = data.uuid;
        //    Email = data.email;
        //    Items = ((IEnumerable<dynamic>)data.items).Select(x => (int)x).ToList();
        //    Guild = data.guild;
        //    HunterCoins = data.hunter_coins;
        //    Wallet = ((IEnumerable<dynamic>)data.wallet).Select(x => new WalletToken((int?)x.token_id, (int)x.item_id)).ToList();
        //    AboutMe = data.about_me;
        //    DiscordLink = data.discord_link;
        //    TwitterLink = data.twitter_link;
        //    TelegramLink = data.telegram_link;
        //    DateJoined = DateTime.ParseExact(data.date_joined, "yyyy MM dd", CultureInfo.InvariantCulture);
        //    ReferralCode = data.referral_code;
        //    Country = data.country;
        //    City = data.city;
        //}
    }

    public class WalletToken
    {
        public int? TokenId { get; }
        public int ItemId { get; }

        public WalletToken(int? tokenId, int itemId)
        {
            TokenId = tokenId;
            ItemId = itemId;
        }
    }
}