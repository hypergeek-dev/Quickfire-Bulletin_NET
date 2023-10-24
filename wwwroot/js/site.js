document.addEventListener("DOMContentLoaded", function () {
    console.log("DOM fully loaded and parsed");

    Array.from(document.querySelectorAll(".add-comment-btn")).forEach(function (btn) {
        btn.addEventListener("click", function () {
            console.log("Add Comment Button Clicked");
            $('#addCommentModal').modal('show');
        });
    });

    Array.from(document.querySelectorAll(".edit-comment-btn")).forEach(function (btn) {
        btn.addEventListener("click", function () {
            const commentId = this.getAttribute("data-comment-id");
            console.log("Edit Comment Button Clicked, Comment ID: ", commentId);
        });
    });

    Array.from(document.querySelectorAll(".delete-comment-btn")).forEach(function (btn) {
        btn.addEventListener("click", function () {
            const commentId = this.getAttribute("data-comment-id");
            console.log("Delete Comment Button Clicked, Comment ID: ", commentId);
        });
    });
});