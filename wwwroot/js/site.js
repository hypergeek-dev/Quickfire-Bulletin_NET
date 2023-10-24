document.addEventListener("DOMContentLoaded", function () {
    let isEditMode = false;
    let currentCommentId = null;

    const articleDiv = document.querySelector('.article');
    const articleId = articleDiv ? articleDiv.getAttribute('data-article-id') : null;
    const myModal = new bootstrap.Modal(document.getElementById('addCommentModal'));

    Array.from(document.querySelectorAll(".add-comment-btn")).forEach(function (btn) {
        btn.addEventListener("click", function () {
            isEditMode = false;
            currentCommentId = null;
            document.getElementById('commentContent').value = '';
            myModal.show();
        });
    });

    Array.from(document.querySelectorAll(".edit-comment-btn")).forEach(function (btn) {
        btn.addEventListener("click", function () {
            isEditMode = true;
            currentCommentId = this.getAttribute("data-comment-id");

            $.get(`/Home/EditComment?commentId=${currentCommentId}`, function (data) {
                document.getElementById('commentContent').value = data.commentContent;
            });

            myModal.show();
        });
    });

    Array.from(document.querySelectorAll(".delete-comment-btn")).forEach(function (btn) {
        btn.addEventListener("click", function () {
            const commentId = this.getAttribute("data-comment-id");

            $.post("/Home/DeleteComment", { commentId: commentId }, function (response) {
                if (response.success) {
                    const commentDiv = document.querySelector(`[data-comment-id="${commentId}"]`).closest(".comment");
                    commentDiv.remove();
                    myModal.hide(); // Explicitly hiding the modal
                    location.reload();
                }
            });
        });
    });

    document.getElementById('saveCommentBtn').addEventListener('click', function () {
        const commentContent = document.getElementById('commentContent').value;

        if (isEditMode) {
            $.post("/Home/EditComment", { commentId: currentCommentId, newContent: commentContent }, function (response) {
                if (response.success) {
                    myModal.hide(); // Explicitly hiding the modal
                    location.reload();
                }
            });
        } else {
            if (articleId) {
                $.post("/Home/AddComment", { articleId: articleId, content: commentContent }, function (response) {
                    if (response.success) {
                        myModal.hide(); // Explicitly hiding the modal
                        location.reload();
                    }
                });
            } else {
                console.error("Article ID is not available.");
            }
        }
    });

    // Hide modal on close button
    document.querySelector("[data-dismiss='modal']").addEventListener('click', function () {
        myModal.hide(); // Explicitly hiding the modal