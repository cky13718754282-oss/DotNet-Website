using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Geekspace.Models;

namespace Geekspace.Data
{
    // Populates the database with sample categories and learning
    // resources on first run, so the site has meaningful content to
    // browse and demo without requiring manual data entry through
    // the admin panel. Safe to call on every startup — it checks
    // whether data already exists before inserting anything.
    public static class SeedData
    {
        public static async Task InitializeAsync(ApplicationDbContext context)
        {
            if (await context.Categories.AnyAsync())
            {
                // Data already seeded — do nothing.
                return;
            }

            var categories = new[]
            {
                new Category
                {
                    Name = "Cybersecurity Basics",
                    Description = "Foundational concepts every security learner should know: threat models, common attack types, and defensive thinking."
                },
                new Category
                {
                    Name = "CTF & Practical Hacking",
                    Description = "Capture the Flag challenges, walkthroughs, and hands-on exercises covering web, binary, and network exploitation."
                },
                new Category
                {
                    Name = "Networking & Protocols",
                    Description = "How data actually moves: TCP/IP, DNS, routing, and the protocols that security tools rely on."
                },
                new Category
                {
                    Name = "Programming for Security",
                    Description = "Scripting and programming skills applied to security tooling, automation, and exploit development."
                }
            };

            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();

            var basics = categories[0];
            var ctf = categories[1];
            var networking = categories[2];
            var programming = categories[3];

            var resources = new[]
            {
                new LearningResource
                {
                    Title = "What is CTF? An Introduction",
                    Description = "A beginner-friendly walkthrough explaining what Capture the Flag competitions are and how to get started.",
                    Content = "Capture the Flag (CTF) competitions are gamified security challenges where participants find hidden 'flags' by exploiting vulnerabilities, solving puzzles, or reverse-engineering software. This resource covers the main categories: web exploitation, cryptography, reverse engineering, forensics, and binary exploitation (pwn).",
                    Type = ResourceType.Video,
                    MediaUrl = "/media/videos/What_is_CTF-LiveOverflow.mp4",
                    CategoryId = ctf.Id,
                    CreatedDate = DateTime.Now.AddDays(-10),
                    IsPublished = true
                },
                new LearningResource
                {
                    Title = "Understanding the CIA Triad",
                    Description = "Confidentiality, Integrity, and Availability — the three pillars every security decision is built on.",
                    Content = "The CIA Triad is the foundational model for information security. Confidentiality ensures data is only accessible to authorized parties. Integrity ensures data has not been tampered with. Availability ensures systems remain accessible when needed. This article explores real-world examples of each principle in action.",
                    Type = ResourceType.Article,
                    MediaUrl = "/media/images/ctf-banner.jpg",
                    CategoryId = basics.Id,
                    CreatedDate = DateTime.Now.AddDays(-8),
                    IsPublished = true
                },
                new LearningResource
                {
                    Title = "TCP/IP Fundamentals Self-Assessment",
                    Description = "Complete a five-question knowledge check covering ports, DNS, reliable delivery, and the TCP three-way handshake.",
                    Content = "Use the interactive assessment below to check your understanding of TCP/IP fundamentals. Answer all five questions, submit your responses, and review the explanation shown beneath each answer. Your score is calculated in the browser and is not stored in the database.",
                    Type = ResourceType.SelfAssessment,
                    CategoryId = networking.Id,
                    CreatedDate = DateTime.Now.AddDays(-6),
                    IsPublished = true
                },
                new LearningResource
                {
                    Title = "Virtual Lab: Setting Up a Home Pentest Environment",
                    Description = "Follow a six-step guided lab to prepare an isolated and recoverable environment for authorised security practice.",
                    Content = "Work through the practical checklist below on your own computer. You will prepare a virtualisation platform, create an isolated network, import practice machines, verify private connectivity, and create a clean snapshot. Progress is saved only in this browser. Always practise only on systems you own or are explicitly authorised to test.",
                    Type = ResourceType.VirtualLab,
                    CategoryId = ctf.Id,
                    CreatedDate = DateTime.Now.AddDays(-4),
                    IsPublished = true
                },
                new LearningResource
                {
                    Title = "Python Scripting for Security Automation",
                    Description = "Learn how Python is used to automate reconnaissance, parse logs, and build simple security tools.",
                    Content = "Python is one of the most widely used languages in security tooling due to its readability and extensive library ecosystem. This resource introduces scripting patterns for automating repetitive tasks, parsing structured log data, and interacting with network sockets.",
                    Type = ResourceType.Article,
                    CategoryId = programming.Id,
                    CreatedDate = DateTime.Now.AddDays(-2),
                    IsPublished = true
                },
                new LearningResource
                {
                    Title = "Simulation: Phishing Email Detection",
                    Description = "Analyse three realistic inbox scenarios, classify each message, and learn the warning signs behind every decision.",
                    Content = "The interactive inbox challenge below presents three realistic messages. Inspect the sender domain, wording, requested action, and destination link before deciding whether each email is phishing or legitimate. Immediate feedback explains the security signals, and a final score summarises your performance.",
                    Type = ResourceType.Simulation,
                    CategoryId = basics.Id,
                    CreatedDate = DateTime.Now.AddDays(-1),
                    IsPublished = true
                }
            };

            context.LearningResources.AddRange(resources);
            await context.SaveChangesAsync();
        }
    }
}
