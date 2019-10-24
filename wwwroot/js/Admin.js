$(document).ready(function () {
	$('button').button();

	$('.ToggleLeftPane').click(function () {
		if ($('#LeftPane').hasClass('visible')) {
			$('#LeftPane').removeClass('visible');
			$('#LeftPane').hide("slide", { direction: "left" }, "slow");
		} else {
			$('#LeftPane').addClass('visible');
			$('#LeftPane').show("slide", { direction: "left" }, "slow");
		}
	});
});

