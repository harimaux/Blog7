//FROALA rich text editor
let editor = new FroalaEditor('#richTextEditor', {
    heightMin: 300,

});


//Way of adding values in code
//editor.opts.height = 200;
//editor.size.refresh();


//SELECT2
$(document).ready(function () {
    $('.SELECTmulti').select2({
        //placeholder: "Select category..",
        initSelection: function (element, callback) {
        }
    });
});

//Error on blank fields
let labelTitle = document.querySelector('.labelTitle');
let inputTitle = document.querySelector('.inputTitle');

inputTitle.addEventListener('mouseup', () => {
    labelTitle.innerHTML = "Title";
    labelTitle.style.color = "unset";
});

let labelCategory = document.querySelector('.labelCategory');
let selectCategory = document.querySelector('.selectCategory');

selectCategory.addEventListener('mouseup', () => {
    labelCategory.innerHTML = "Category";
    labelCategory.style.color = "unset";
});

let labelEditor = document.querySelector('.labelEditor');
let textareaEditor = document.querySelector('.textareaEditor');

textareaEditor.addEventListener('mouseup', () => {
    labelEditor.innerHTML = "Your Post";
    labelEditor.style.color = "unset";
});

// Get all the delete buttons
let deleteButtons = document.querySelectorAll('.displayPostCustomDeletePost');

//Form sendPostFormEdit
var form = document.querySelector('.sendPostForm');
form.onsubmit = async function (e) {

    console.log("Run")

    e.preventDefault();

    $(".postEmptyErrorBox").remove();

    const postTitle = document.querySelector('input[name=Title]').value;
    const category = $('.SELECTmulti').select2("val");
    const richTextContent = document.querySelector('textarea[name=richContent]').value;

    //Errors on empty input
    if (postTitle == "") {

        labelTitle.innerHTML = "Please enter a title for your post.";
        labelTitle.style.color = "red";
        labelTitle.scrollIntoView({ behavior: "smooth", block: "end", inline: "nearest" });

        return false;
    }

    if (category == "") {
        labelCategory.innerHTML = "Please choose a category for your post.";
        labelCategory.style.color = "red";
        labelCategory.scrollIntoView({ behavior: "smooth", block: "end", inline: "nearest" });

        return false;
    }

    if (richTextContent == "") {
        labelEditor.innerHTML = "Please enter a content of your post.";
        labelEditor.style.color = "red";
        labelEditor.scrollIntoView({ behavior: "smooth", block: "end", inline: "nearest" });

        return false;
    }

    //This code defines an asynchronous function convertToBase64 that takes a URL as a parameter.It uses the Fetch API to fetch the resource at the given URL, retrieves the response as a blob, and then converts the blob to a base64 - encoded string using a FileReader.The function returns a promise that resolves with the base64 string.
    const convertToBase64 = async (url) => {
        const response = await fetch(url);
        const blob = await response.blob();
        return new Promise((resolve, reject) => {
            const reader = new FileReader();
            reader.onloadend = () => resolve(reader.result);
            reader.onerror = reject;
            reader.readAsDataURL(blob);
        });
    };


    //These lines define an empty array promises and use the replace() method with a regular expression to iterate over the img tags in the richTextContent string. For each img tag, it extracts the value of the src attribute using a regex match. If the src value doesn't start with 'http' or 'https', indicating it's a local file, it calls the convertToBase64 function with the src value and adds the resulting promise to the promises array. The replace() method returns the updated content with the original img tags unchanged.
    const promises = [];
    const updatedContent = richTextContent.replace(/<img([^>]+)>/g, (match, p1) => {
        const srcMatch = p1.match(/src\s*=\s*(['"])(.*?)\1/);
        if (srcMatch) {
            const src = srcMatch[2];
            if (!src.startsWith('http') && !src.startsWith('https')) {
                const base64Promise = convertToBase64(src);
                promises.push(base64Promise);
                return match;
            }
        }
        return match;
    });

    //These lines use Promise.all() to await all the promises in the promises array. This ensures that all the base64 conversions are completed before further processing. The resulting array of base64 strings is stored in the base64Array variable. The code then uses the replace() method again to iterate over the img tags in the updatedContent string. It checks if the src value indicates a local file, and if so, replaces it with the corresponding base64 string from the base64Array. This updates the src attribute of the img tags with the base64-encoded images.
    const base64Array = await Promise.all(promises);
    let base64Index = 0;
    const updatedContentWithBase64 = updatedContent.replace(/<img([^>]+)>/g, (match, p1) => {
        const srcMatch = p1.match(/src\s*=\s*(['"])(.*?)\1/);
        if (srcMatch) {
            const src = srcMatch[2];
            if (!src.startsWith('http') && !src.startsWith('https')) {
                const base64 = base64Array[base64Index];
                base64Index++;
                return match.replace(src, base64);
            }
        }
        return match;
    });

    var form = $(this);
    var url = form.attr('action');

    const formData = new FormData();
    formData.append('RichContent', updatedContentWithBase64);
    formData.append('Title', postTitle);
    formData.append('Category', category)

    $.ajax({
        url: url,
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (data) {

            //Clears inputs
            let clearPostTitle = document.querySelector('input[name=Title]');
            clearPostTitle.value = "";

            $(".SELECTmulti").val('').trigger('change');

            let clearPostContent = document.querySelector('.fr-element');
            clearPostContent.innerHTML = "";

            // Create a jQuery element from the data
            const newPost = $(data);

            // Insert the new post at the top of the list
            try {
                $('#partialViewContainer').prepend(newPost);

                // Scroll to post after a short delay to allow time for rendering
                setTimeout(function () {
                    let firstPostInList = document.querySelector('.displayPostCustomBox');
                    firstPostInList.scrollIntoView({ behavior: "smooth", block: "center", inline: "nearest" });
                }, 100);


                if ($('#partialViewContainer .displayPostCustomBox').length > 3) {

                    $('#partialViewContainer .displayPostCustomBox').last().remove();
                }

            } catch (error) {
                alert(error)
            }

        },
        error: function (xhr, status, error) {
            var errorMessage = xhr.responseText || 'An error occurred.';
            alert('Error: ' + errorMessage);
            console.log(errorMessage);
        }
    });

    return false;
};



//DELETE POST

const partialViewContainer = document.getElementById('partialViewContainer');

if (partialViewContainer != null) {

    partialViewContainer.addEventListener('click', function (e) {

        if (e.target.matches('.displayPostCustomDeletePost')) {

            const deleteButton = e.target;
            const postId = parseInt(deleteButton.getAttribute('data-value'));

            const confirmBox = deleteButton.nextElementSibling;

            const deletePost = confirmBox.querySelector('.acceptDeletePost');
            const cancelDelete = confirmBox.querySelector('.cancelDeletePost');

            //Get main box
            const postContainer = deleteButton.closest('.displayPostCustomBox');
            //const dateDisplay = postContainer.querySelector('.displayPostCustomDate');

            $(deleteButton).effect("bounce", { times: 2 }, "fast", function () {

                deleteButton.classList.add('hideDeleteButton');
                confirmBox.style.display = "flex";

            });

            //Remove styles
            deleteButton.classList.remove('showDeleteButton');

            //Cancel delete post
            cancelDelete.addEventListener('click', () => {

                deleteButton.classList.remove('hideDeleteButton');
                deleteButton.classList.add('showDeleteButton');
                confirmBox.style.display = "none";
            });

            //Delete post
            deletePost.addEventListener('click', () => {

                $(postContainer).effect("shake", { times: 3 }, "slow", function () {

                    postContainer.classList.add('removePostBox');

                    var deleteUrl = baseUrl + `?postId=${postId}`;

                    $.ajax({
                        url: deleteUrl,
                        type: 'POST',
                        success: function (data) {
                            postContainer.remove();
                        },
                        error: function (xhr, status, error) {
                            var errorMessage = xhr.responseText || 'An error occurred.';
                            alert('Error: ' + errorMessage);
                        }
                    });

                });
            });

        };

    });

}
