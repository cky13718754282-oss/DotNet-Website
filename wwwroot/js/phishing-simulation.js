(function () {
    "use strict";

    var module = document.querySelector("[data-phishing-simulation]");
    if (!module) return;

    var scenarios = [
        {
            sender: "APU IT Support <support@apu-security-reset.com>",
            subject: "URGENT: Your student account will be disabled today",
            body: "<p>We detected unusual activity on your account. To avoid immediate suspension, verify your username and password using the secure link below.</p><p><strong>Verify now — your access expires in 30 minutes.</strong></p>",
            link: "https://apu-security-reset.com/verify",
            answer: "phishing",
            explanation: "This message impersonates university IT support and attempts to steal login credentials.",
            signals: [
                "The sender uses apu-security-reset.com instead of an official APU domain.",
                "The message creates extreme urgency to prevent careful checking.",
                "It asks for credentials through an external link."
            ]
        },
        {
            sender: "APU Moodle <noreply@moodle.apu.edu.my>",
            subject: "New course announcement: Web Applications",
            body: "<p>Your lecturer posted a new announcement in CT050-3-2-WAPP.</p><p>Sign in through the normal Moodle portal to read the announcement. This notification does not request your password.</p>",
            link: "https://moodle.apu.edu.my/course/view.php",
            answer: "legitimate",
            explanation: "This notification is consistent with a normal learning-platform message.",
            signals: [
                "The sender and destination use the official apu.edu.my domain.",
                "The message does not request sensitive information.",
                "There is no threatening deadline or unexpected attachment."
            ]
        },
        {
            sender: "Student Rewards <winner@campus-gift-prize.net>",
            subject: "Congratulations — claim your free laptop",
            body: "<p>Your university email was selected for an exclusive student reward.</p><p>Pay a small delivery fee and confirm your bank card details to receive your laptop today.</p>",
            link: "http://campus-gift-prize.net/claim-card",
            answer: "phishing",
            explanation: "This is a prize scam designed to collect payment-card and personal information.",
            signals: [
                "The offer is unexpected and sounds too good to be true.",
                "The sender is unrelated to the university.",
                "The link uses plain HTTP and requests financial information."
            ]
        }
    ];

    var index = 0;
    var score = 0;
    var answered = false;

    var subject = module.querySelector("[data-email-subject]");
    var sender = module.querySelector("[data-email-sender]");
    var body = module.querySelector("[data-email-body]");
    var link = module.querySelector("[data-email-link]");
    var avatar = module.querySelector("[data-email-avatar]");
    var progress = module.querySelector("[data-simulation-progress]");
    var scoreOutput = module.querySelector("[data-simulation-score]");
    var feedback = module.querySelector("[data-simulation-feedback]");
    var feedbackIcon = module.querySelector("[data-feedback-icon]");
    var feedbackTitle = module.querySelector("[data-feedback-title]");
    var feedbackExplanation = module.querySelector("[data-feedback-explanation]");
    var feedbackSignals = module.querySelector("[data-feedback-signals]");
    var nextButton = module.querySelector("[data-simulation-next]");
    var decisionPanel = module.querySelector("[data-decision-panel]");
    var result = module.querySelector("[data-simulation-result]");
    var resultText = module.querySelector("[data-simulation-result-text]");
    var choiceButtons = Array.from(module.querySelectorAll("[data-simulation-choice]"));

    function renderScenario() {
        var scenario = scenarios[index];
        answered = false;
        subject.textContent = scenario.subject;
        sender.textContent = scenario.sender;
        body.innerHTML = scenario.body;
        link.textContent = scenario.link;
        avatar.textContent = scenario.sender.charAt(0).toUpperCase();
        progress.textContent = "Scenario " + (index + 1) + " of " + scenarios.length;
        scoreOutput.textContent = "Score: " + score;
        feedback.hidden = true;
        result.hidden = true;
        decisionPanel.hidden = false;
        choiceButtons.forEach(function (button) {
            button.disabled = false;
            button.classList.remove("is-correct", "is-wrong");
        });
    }

    function chooseAnswer(event) {
        if (answered) return;
        answered = true;

        var selected = event.currentTarget.dataset.simulationChoice;
        var scenario = scenarios[index];
        var isCorrect = selected === scenario.answer;
        if (isCorrect) score += 1;

        choiceButtons.forEach(function (button) {
            button.disabled = true;
            if (button.dataset.simulationChoice === scenario.answer) {
                button.classList.add("is-correct");
            } else if (button === event.currentTarget) {
                button.classList.add("is-wrong");
            }
        });

        feedbackIcon.textContent = isCorrect ? "✓" : "!";
        feedbackTitle.textContent = isCorrect ? "Good decision" : "Not quite";
        feedbackExplanation.textContent = scenario.explanation;
        feedbackSignals.innerHTML = "";
        scenario.signals.forEach(function (signal) {
            var item = document.createElement("li");
            item.textContent = signal;
            feedbackSignals.appendChild(item);
        });
        feedback.classList.toggle("feedback-correct", isCorrect);
        feedback.classList.toggle("feedback-wrong", !isCorrect);
        nextButton.textContent = index === scenarios.length - 1 ? "View final score" : "Next scenario";
        scoreOutput.textContent = "Score: " + score;
        feedback.hidden = false;
        feedback.scrollIntoView({ behavior: "smooth", block: "nearest" });
    }

    function showNext() {
        if (index < scenarios.length - 1) {
            index += 1;
            renderScenario();
            module.scrollIntoView({ behavior: "smooth", block: "start" });
            return;
        }

        decisionPanel.hidden = true;
        feedback.hidden = true;
        result.hidden = false;
        var message = score === scenarios.length
            ? "Excellent — you scored 3 / 3 and identified every message correctly."
            : score === 2
                ? "Good work — you scored 2 / 3. Review the warning signs before trying again."
                : "You scored " + score + " / 3. Slow down and verify the sender, wording and link destination.";
        resultText.textContent = message;
        result.scrollIntoView({ behavior: "smooth", block: "center" });
    }

    function restart() {
        index = 0;
        score = 0;
        renderScenario();
        module.scrollIntoView({ behavior: "smooth", block: "start" });
    }

    choiceButtons.forEach(function (button) {
        button.addEventListener("click", chooseAnswer);
    });
    nextButton.addEventListener("click", showNext);
    module.querySelector("[data-simulation-restart]").addEventListener("click", restart);
    renderScenario();
})();
