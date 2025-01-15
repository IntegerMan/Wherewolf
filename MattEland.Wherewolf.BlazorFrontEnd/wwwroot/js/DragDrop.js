window.dragDropInterop = {
    setDragData: function(event, data) {
        if (event instanceof DragEvent) {
            console.log('Drag event is DragEvent', event);
        }
        else {
            console.error("Dragging Event is not a DragEvent, it is a", event.constructor.name, event);
        }
        console.log('Setting data to ', data, event);
        event.dataTransfer.setData("text/plain", data);
    },
    handleDrop:  function(event) {
        let data;
        if (event instanceof DragEvent) {
            event.preventDefault();
            data = event.dataTransfer.getData("text/plain");
        } else {
            console.error("Event is not a DragEvent, it is a", event.constructor.name, event);
        }

        if (data) {
            console.log('Invoking HandleDropped with', data);
            DotNet.invokeMethodAsync('MattEland.Wherewolf.BlazorFrontEnd', 'HandleDropped', data)
                .catch(err => console.error(err));
        } else {
            console.error("No data found in event.dataTransfer", event);
        }
    }
};