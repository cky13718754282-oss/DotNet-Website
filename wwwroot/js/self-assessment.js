(function () {
    "use strict";

    var module = document.querySelector("[data-self-assessment]");
    if (!module) return;

    var form = module.querySelector("[data-assessment-form]");
    var questions = Array.from(module.querySelectorAll(".assessment-question"));
    var validation = module.querySelector("[data-assessment-validation]");
    var result = module.querySelector("[data-assessment-result]");
    var scoreOutput = module.querySelector("[data-assessment-score]");
    var heading = module.querySelector("[data-assessment-heading]");
    var mark = module.querySelector("[data-assessment-mark]");

    function clearReview() {
        questions.forEach(function (question) {
            question.classList.remove("answer-correct", "answer-wrong");
            question.querySelector(".answer-explanation").hidden = true;
            var answerState = question.querySelector("[data-answer-state]");
            answerState.hidden = true;
            answerState.textContent = "";
            answerState.classList.remove("is-correct", "is-wrong");
            question.querySelectorAll("label").forEach(function (label) {
                label.classList.remove("selected-correct", "selected-wrong", "correct-option");
            });
        });
        validation.hidden = true;
        result.hidden = true;
    }

    form.addEventListener("submit", function (event) {
        event.preventDefault();
        clearReview();

        var unanswered = questions.some(function (question) {
            return !question.querySelector("input:checked");
        });

        if (unanswered) {
            validation.hidden = false;
            validation.scrollIntoView({ behavior: "smooth", block: "center" });
            return;
        }

        var score = 0;
        questions.forEach(function (question) {
            var selected = question.querySelector("input:checked");
            var correctValue = question.dataset.correct;
            var isCorrect = selected.value === correctValue;
            if (isCorrect) score += 1;

            question.classList.add(isCorrect ? "answer-correct" : "answer-wrong");
            selected.closest("label").classList.add(isCorrect ? "selected-correct" : "selected-wrong");
            var correctInput = question.querySelector('input[value="' + correctValue + '"]');
            if (correctInput) correctInput.closest("label").classList.add("correct-option");
            var answerState = question.querySelector("[data-answer-state]");
            answerState.textContent = isCorrect ? "✓ Correct" : "✕ Incorrect";
            answerState.classList.add(isCorrect ? "is-correct" : "is-wrong");
            answerState.hidden = false;
            question.querySelector(".answer-explanation").hidden = false;
        });

        var percentage = Math.round((score / questions.length) * 100);
        heading.textContent = percentage >= 80 ? "Strong understanding" : percentage >= 60 ? "Good start" : "Review recommended";
        mark.textContent = percentage >= 60 ? "✓" : "↻";
        scoreOutput.textContent = "You scored " + score + " / " + questions.length + " (" + percentage + "%). " +
            (percentage >= 80
                ? "You have a solid grasp of the TCP/IP fundamentals covered here."
                : "Review the explanations below, then reset the assessment and try again.");
        result.hidden = false;
        result.scrollIntoView({ behavior: "smooth", block: "center" });
    });

    module.querySelector("[data-assessment-reset]").addEventListener("click", function () {
        form.reset();
        clearReview();
        module.scrollIntoView({ behavior: "smooth", block: "start" });
    });

    module.querySelector("[data-assessment-review]").addEventListener("click", function () {
        questions[0].scrollIntoView({ behavior: "smooth", block: "start" });
    });
})();
