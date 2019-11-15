$(document).ready(function () {    
    resizeAppPartDynamically();
    loadEmployeesToEmployeeTable();
});

var paramStr = document.URL.substring(document.URL.indexOf('?'));

function loadEmployeesToEmployeeTable() {
    let url = "/api/employees";
    let body = $('#tbl-employee tbody');
    $.ajax({
        type: 'GET',
        url: url
    }).done(function (employees) {
        body.empty();

        for (let emp of employees) {
            let row = $('<tr></tr>', {
                html: `
                <td>${emp.ID}</td>
                <td>${emp.NationalID}</td>
                <td>${emp.Name}</td>
                <td>${emp.JobTitle}</td>
                <td>
                    <button data-action="publish"
                            data-emp-id="${emp.ID}" 
                            class="btn btn-success btn-flat">Publish</button>
                    <button data-action="delete"
                            data-emp-id="${emp.ID}" 
                            class="btn btn-danger btn-flat">Delete</button>
                </td>
            `
            });
            row.data('model', emp);
            body.append(row);
        }

        $('button[data-action="publish"]').on('click', function (e) {
            let row = $(e.target).parents('tr')[0];
            let selectedEmp = $(row).data('model');
            //let url = "/api/provision/publish-employee-item";
            let url = `/Employee/PublishEmployeeItem${paramStr}`;
            console.log(selectedEmp);
            $.ajax({
                type: 'POST',
                url: url,
                data: selectedEmp,
                dataType: "json",
            })
            .done(function (result) {
                if (result.Success) {
                    alert(result.Message);
                }
                else {
                    alert(result.Message);
                }
            })
        });

        $('button[data-action="delete"]').on('click', function (e) {
            let row = $(e.target).parents('tr')[0];
            let selectedEmp = $(row).data('model');
            let url = `/Employee/RemoveEmployeeItem${paramStr}`;
            console.log(selectedEmp);
            $.ajax({
                type: 'POST',
                url: url,
                data: selectedEmp,
                dataType: "json",
            })
                .done(function (result) {
                    if (result.Success) {
                        alert(result.Message);
                    }
                    else {
                        alert(result.Message);
                    }
                })
        });
    });
}

function resizeAppPartDynamically() {
    window.reSize = window.reSize || {};

    //Accordion app responsive width and height
    reSize.AppPart = {
        senderId: '',      // the App Part provides a Sender Id in the URL parameters,
        // every time the App Part is loaded, a new Id is generated.
        // The Sender Id identifies the rendered App Part.
        previousHeight: 0, // the height
        minHeight: 0,      // the minimal allowed height
        firstResize: true, // On the first call of the resize the App Part might be
        // already too small for the content, so force to resize.

        init: function () {
            // parse the URL parameters and get the Sender Id
            var params = document.URL.split("?")[1].split("&");
            for (var i = 0; i < params.length; i = i + 1) {
                var param = params[i].split("=");
                if (param[0].toLowerCase() == "senderid")
                    this.senderId = decodeURIComponent(param[1]);
            }

            // find the height of the app part, uses it as the minimal allowed height
            this.previousHeight = this.minHeight = $('body').height();

            // display the Sender Id
            $('#senderId').text(this.senderId);

            // make an initial resize (good if the content is already bigger than the
            // App Part)
            this.autoSize();
        },

        autoSize: function () {
            // Post the request to resize the App Part, but just if has to make a resize
            var step = 30, // the recommended increment step is of 30px. Source:// http://msdn.microsoft.com/en-us/library/jj220046.aspx
                height = $('body').height() + 7,  // the App Part height // (now it's 7px more than the body)
                newHeight,                        // the new App Part height
                contentHeight = $('.accordion-Wrapper').height(), //Specify your name of parent div
                resizeMessage = '<message senderId={Sender_ID}>resize({Width}, {Height})</message>';

            // if the content height is smaller than the App Part's height,
            // shrink the app part, but just until the minimal allowed height
            if (contentHeight < height - step && contentHeight >= this.minHeight) {
                height = contentHeight;
            }

            // if the content is bigger or smaller then the App Part
            // (or is the first resize)
            if (this.previousHeight !== height || this.firstResize === true) {
                // perform the resizing
                newHeight = contentHeight;

                // set the parameters
                resizeMessage = resizeMessage.replace("{Sender_ID}", this.senderId);
                resizeMessage = resizeMessage.replace("{Height}", "100%");
                resizeMessage = resizeMessage.replace("{Width}", "100%");
                // we are not changing the width here, but we could

                // post the message
                window.parent.postMessage(resizeMessage, "*");

                // memorize the height
                this.previousHeight = newHeight;

                // further resizes are not the first ones
                this.firstResize = false;
            }
        }
    };

    //App responsive on load
    reSize.AppPart.init();

    setTimeout(function () {
        reSize.AppPart.autoSize();
    }, 500);
}


