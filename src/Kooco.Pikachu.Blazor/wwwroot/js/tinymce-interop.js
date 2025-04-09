window.initTinyMCE = function (element, dotnetRef) {
    tinymce.init({
        target: element,
        height: 500,
        menubar: false,
        language: 'zh_TW',
        language_url: 'https://cdn.jsdelivr.net/npm/tinymce-i18n@23.7.24/langs5/zh_TW.min.js',
        plugins: 'advlist lists link image code table media textcolor searchreplace autolink directionality emoticons hr paste wordcount autoresize fullscreen preview print',
        toolbar: [
            'undo redo | formatselect | fontselect fontsizeselect | bold italic underline strikethrough | alignleft aligncenter alignright alignjustify',
            'forecolor backcolor | bullist numlist | outdent indent | link image media table emoticons hr | searchreplace | fullscreen preview print | code'
        ],
        media_live_embeds: true,
        media_url_resolver: function (data, resolve) {
            resolve({
                html: '<iframe src="' + data.url + '" width="560" height="315" frameborder="0" allowfullscreen></iframe>'
            });
        },
        fontsize_formats: '12px 14px 16px 18px 24px 36px 48px',
        max_chars: 65535,
        setup: function (editor) {
            editor.on('change', function () {
                const content = editor.getContent();
                const textLength = editor.getContent({ format: 'text' }).length;
                
                if (textLength > 65535) {
                    editor.notificationManager.open({
                        text: '內容超過65535字元限制',
                        type: 'warning'
                    });
                    return;
                }
                
                dotnetRef.invokeMethodAsync('OnEditorChange', content);
            });
        }
    });
}; 