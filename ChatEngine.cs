using System;
using System.Collections.Generic;

namespace CybersecurityBot
{
    class ChatEngine
    {
        public string UserName { get; private set; } = "Friend";

        // Memory store
        private string favoriteTopic = "";
        private string lastTopic = "";

        private Random rng = new Random();

        // Random responses per topic
        private Dictionary<string, List<string>> randomResponses = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
        {
            {
                "phishing", new List<string>
                {
                    "Be cautious of emails asking for personal information. Scammers often disguise themselves as trusted organisations.",
                    "Always check the sender's email address carefully. Phishing emails often use slight misspellings of real domains.",
                    "Never click links in unexpected emails. Go directly to the website by typing the address in your browser."
                }
            },
            {
                "password", new List<string>
                {
                    "Use a mix of letters, numbers, and symbols. Aim for at least 12 characters and never reuse passwords.",
                    "Consider using a password manager to generate and store strong, unique passwords for each account.",
                    "Avoid using personal info like birthdays or pet names in passwords — these are easy to guess."
                }
            },
            {
                "scam", new List<string>
                {
                    "If something sounds too good to be true, it probably is. Verify before you trust.",
                    "Scammers often create urgency to make you act quickly. Pause and think before responding.",
                    "Never send money or personal info to someone you haven't verified through official channels."
                }
            },
            {
                "privacy", new List<string>
                {
                    "Review the privacy settings on your social media accounts regularly.",
                    "Be mindful of what personal information you share online — it can be used against you.",
                    "Use a VPN when connecting to public Wi-Fi to protect your data from eavesdroppers."
                }
            },
            {
                "malware", new List<string>
                {
                    "Keep your antivirus software updated and run regular scans on your device.",
                    "Avoid downloading software from unofficial sources — it may contain hidden malware.",
                    "Ransomware locks your files until you pay. Regular backups are your best defence."
                }
            },
            {
                "safe browsing", new List<string>
                {
                    "Stick to HTTPS websites and look for the padlock icon before entering any personal details.",
                    "Avoid clicking pop-up ads — they can lead to malicious sites or trigger downloads.",
                    "Keep your browser and plugins updated to protect against known vulnerabilities."
                }
            }
        };

        // Simple single responses for general questions
        private Dictionary<string, string> simpleResponses = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "how are you",     "Doing great, thanks for asking! Ready to help you stay safe online." },
            { "what's your purpose", "I'm here to teach you about cybersecurity — phishing, passwords, safe browsing, and more." },
            { "what can i ask",  "You can ask me about passwords, phishing, scams, privacy, malware, and safe browsing." },
            { "hello",           "Hey! How can I help you with cybersecurity today?" },
            { "hi",              "Hi there! Ask me anything about staying safe online." },
            { "2fa",             "Two-factor authentication adds a second verification step after your password. It's one of the best ways to protect your accounts." },
            { "two factor",      "Two-factor authentication adds a second verification step after your password. It's one of the best ways to protect your accounts." },
            { "social engineering", "Social engineering tricks people into giving away sensitive info. Be wary of unexpected calls or emails asking for details." },
            { "bye",             "Stay safe out there! Goodbye." },
            { "goodbye",         "Stay safe out there! Goodbye." }
        };

        // Sentiment keywords
        private List<string> worriedWords   = new List<string> { "worried", "scared", "nervous", "anxious", "afraid", "fear" };
        private List<string> frustratedWords = new List<string> { "frustrated", "annoyed", "angry", "confused", "lost", "stuck" };
        private List<string> curiousWords   = new List<string> { "curious", "interested", "wondering", "want to know", "tell me more", "explain" };

        // Conversation flow keywords
        private List<string> moreKeywords = new List<string> { "more", "another tip", "tell me more", "give me another", "explain more", "go on" };

        public void SetUserName(string name)
        {
            UserName = string.IsNullOrWhiteSpace(name) ? "Friend" : name.Trim();
        }

        public string GetResponse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "I didn't catch that. Could you type something?";

            string lower = input.ToLower();

            // Check for "remember my favourite topic" memory feature
            if (lower.Contains("i'm interested in") || lower.Contains("i am interested in") || lower.Contains("my favourite topic"))
            {
                foreach (var key in randomResponses.Keys)
                {
                    if (lower.Contains(key))
                    {
                        favoriteTopic = key;
                        return $"Great! I'll remember that you're interested in {key}. It's a crucial part of staying safe online.";
                    }
                }
            }

            // Reference stored memory
            if (!string.IsNullOrEmpty(favoriteTopic) &&
                (lower.Contains("remember") || lower.Contains("my topic") || lower.Contains("what do i like")))
            {
                return $"As someone interested in {favoriteTopic}, you might want to review the security settings on your accounts and read up on the latest {favoriteTopic} threats.";
            }

            // Sentiment detection — check first, then still give a tip
            string sentimentPrefix = DetectSentiment(lower);

            // Conversation flow: "more", "another tip"
            if (moreKeywords.Exists(k => lower.Contains(k)))
            {
                if (!string.IsNullOrEmpty(lastTopic) && randomResponses.ContainsKey(lastTopic))
                {
                    string tip = GetRandomResponse(lastTopic);
                    return sentimentPrefix + tip;
                }
                return sentimentPrefix + "Sure! What topic would you like more on — passwords, phishing, scams, privacy, or malware?";
            }

            // Keyword recognition for random responses
            foreach (var key in randomResponses.Keys)
            {
                if (lower.Contains(key))
                {
                    lastTopic = key;
                    return sentimentPrefix + GetRandomResponse(key);
                }
            }

            // Simple single responses
            foreach (var key in simpleResponses.Keys)
            {
                if (lower.Contains(key))
                    return sentimentPrefix + simpleResponses[key];
            }

            // Default / error handling fallback
            return "I'm not sure I understand. Could you try rephrasing? You can ask about passwords, phishing, scams, privacy, or safe browsing.";
        }

        private string GetRandomResponse(string topic)
        {
            var list = randomResponses[topic];
            return list[rng.Next(list.Count)];
        }

        private string DetectSentiment(string lower)
        {
            if (worriedWords.Exists(w => lower.Contains(w)))
                return "It's completely understandable to feel that way. You're not alone — many people have the same concerns. " + Environment.NewLine;

            if (frustratedWords.Exists(w => lower.Contains(w)))
                return "I hear you — this stuff can feel overwhelming. Let's break it down together. " + Environment.NewLine;

            if (curiousWords.Exists(w => lower.Contains(w)))
                return "Great that you're curious — that's the first step to staying safe! " + Environment.NewLine;

            return "";
        }
    }
}
