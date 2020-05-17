/**
 * @license Copyright (c) 2003-2017, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function (config) {
    config.toolbarGroups = [
        { name: 'clipboard', groups: ['clipboard', 'undo'] },
        { name: 'editing', groups: ['find', 'selection', 'spellchecker'] },
        { name: 'links' },
        { name: 'insert' },
        { name: 'forms' },
        { name: 'tools' },
        { name: 'document', groups: ['mode', 'document', 'doctools'] },
        { name: 'others' },
        { name: 'basicstyles', groups: ['basicstyles', 'cleanup'] },
        { name: 'paragraph', groups: ['list', 'indent', 'blocks', 'align', 'bidi'] },
        { name: 'styles' },
        { name: 'colors' }
    ];

    config.extraPlugins = 'autoembed,embedsemantic,uploadimage,image2,uploadfile,uploadwidget,widget,lineutils,widgetselection,autolink,embedbase,filetools,notificationaggregator';
    config.imageUploadUrl = '/Admin/Image/Upload?Public=true';
    config.uploadUrl = '/Admin/Image/Upload?Public=true';

    config.removeButtons = 'Underline,Subscript,Superscript';

    config.format_tags = 'p;h1;h2;h3;pre';

    config.removeDialogTabs = 'image:advanced;link:advanced';

    config.enterMode = CKEDITOR.ENTER_BR;

    config.shiftEnterMode = CKEDITOR.ENTER_BR;

    config.protectedSource.push(/\n/g);
    config.protectedSource.push(/"/g);

    config.allowedContent = true;
};