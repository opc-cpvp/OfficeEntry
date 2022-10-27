// ===========================================================================
// classes START
// ===========================================================================
class Queue {
    constructor() {
        this.elements = {};
        this.head = 0;
        this.tail = 0;
    }
    enqueue(element) {
        this.elements[this.tail] = element;
        this.tail++;
    }
    dequeue() {
        const item = this.elements[this.head];
        delete this.elements[this.head];
        this.head++;
        return item;
    }
    peek() {
        return this.elements[this.head];
    }
    average() {
        let sum = 0;
        for (let i = this.head; i < this.tail; i++) {
            sum += this.elements[i];
        }
        return sum / this.length;
    }
    get length() {
        return this.tail - this.head;
    }
    get isEmpty() {
        return this.length === 0;
    }
}

class Position {
    Left = 0;
    Top = 0;

    constructor(x, y) {
        this.Left = x;
        this.Top = y;
    }
}

class Circle {
    Position = new Position();
    Diameter = 0;
    Id = "";
    Name = "";
    Hovering = false;
    Selected = false;
    Grabbed = false;
    Taken = false;
    IsFirstAidAttendant = false;
    IsFloorEmergencyOfficer = false;
    EmployeeFullName = "";

    constructor(x, y, diameter, name, id, selected) {
        this.Position = new Position(x, y);
        this.Diameter = diameter;
        this.Name = name;
        this.Id = id;
        this.Selected = selected;
    }

    /**
     * Axis-aligned bounding box (AABB) collision detetion for a circle and the current mouse position.
     *
     * We are using a square and not a circle to have a margin of error around the circle.
     *
     * @param {MouseState} mouseState
     */
    IsCollidingWithMouse(mouseState) {
        return this.Position.Left < mouseState.X &&
            this.Position.Left + this.Diameter > mouseState.X &&
            this.Position.Top < mouseState.Y &&
            this.Position.Top + this.Diameter > mouseState.Y;
    }
}

class MouseState {
    X = 0;
    Y = 0;
    LeftButton = "up";

    clone() {
        const clone = new MouseState();

        clone.X = this.X;
        clone.Y = this.Y;
        clone.LeftButton = this.LeftButton;

        return clone;
    }
}

class FrameCounter {
    TotalFrames = 0;
    TotalSeconds = 0;
    AverageFramesPerSecond = 0.0;
    CurrentFramesPerSecond = 0.0;
    #MAXIMUM_SAMPLES = 100;
    #_sampleBuffer = new Queue()

    Update(deltaTime) {
        this.CurrentFramesPerSecond = 1.0 / deltaTime;

        this.#_sampleBuffer.enqueue(this.CurrentFramesPerSecond);

        if (this.#_sampleBuffer.length > this.#MAXIMUM_SAMPLES) {
            this.#_sampleBuffer.dequeue();
            this.AverageFramesPerSecond = this.#_sampleBuffer.average();
        }
        else {
            this.AverageFramesPerSecond = this.CurrentFramesPerSecond;
        }

        this.TotalFrames++;
        this.TotalSeconds += deltaTime;

        return true;
    }
}

// class FrameCounter {
//     TotalFrames = 0;
//     TotalSeconds = 0;
//     AverageFramesPerSecond = 0.0;
//     CurrentFramesPerSecond = 0.0;
//     #MAXIMUM_SAMPLES = 100;
//     #_sampleBuffer = [];

//     _sampleBuffer = new Queue()

//     Update(deltaTime) {
//         this.CurrentFramesPerSecond = 1.0 / deltaTime;

//         this.#_sampleBuffer.push(this.CurrentFramesPerSecond);

//         if (this.#_sampleBuffer.length > this.#MAXIMUM_SAMPLES) {
//             this.#_sampleBuffer.shift();
//             this.AverageFramesPerSecond = this.#_sampleBuffer.reduce((a, b) => a + b, 0) / this.#_sampleBuffer.length;
//         }
//         else {
//             this.AverageFramesPerSecond = this.CurrentFramesPerSecond;
//         }

//         this.TotalFrames++;
//         this.TotalSeconds += deltaTime;

//         return true;
//     }
// }
// ===========================================================================
// classes END
// ===========================================================================


// ===========================================================================
// Input manager START
// ===========================================================================
function onMouseMove(e) {
    currentMouseState.X = e.offsetX;
    currentMouseState.Y = e.offsetY;
};

function onMouseDown(e) {
    currentMouseState.LeftButton = "down";
}

function onMouseUp(e) {
    currentMouseState.LeftButton = "up";
}

function onTouchStart(e) {
    currentMouseState.LeftButton = "down";
}

function onTouchEnd(e) {
    currentMouseState.LeftButton = "up";
}
// ===========================================================================
// Input manager END
// ===========================================================================

// https://jacksonlab.agronomy.wisc.edu/2016/05/23/15-level-colorblind-friendly-palette/
const availableBackgroundImage = new Image();
const availableHoveredBackgroundImage = new Image();
const takenBackgroundImage = new Image();
const selectedBackgroundImage = new Image();
const firstAidAttendantImage = new Image();
const floorEmergencyOfficerImage = new Image();
const multipleRolesImage = new Image();
const userImage = new Image();

const floorplan = new Image();
const circles = [];
const _diameter = 24;

let shouldRender = false;
let canEdit = false;
let currentMouseState = new MouseState();
let previousMouseState = new MouseState();
let previousTimeStamp = 0.0;
let _frameCounter = new FrameCounter();

// const grabCountDownBufferDefault = 35;
// let grabCountDownBuffer = grabCountDownBufferDefault;

function resetState() {
    shouldRender = false;

    currentMouseState = new MouseState();
    previousMouseState = new MouseState();

    previousTimeStamp = 0.0;
    _frameCounter = new FrameCounter();

    while (circles.length > 0) {
        circles.pop();
    }
}

let spyingContact = "";

async function Update(deltaTime) {
    // set the current selected circle
    if (previousMouseState.LeftButton === "down" && currentMouseState.LeftButton === "up") {

        // circles.forEach(circle => {
        //     circle.Grabbed = false;
        // });

        // // reset last the buffer
        // grabCountDownBuffer = grabCountDownBufferDefault

        circles.forEach(circle => {
            circle.Selected = circle.IsCollidingWithMouse(currentMouseState);
        });

        const selectedCircles = circles.filter(x => x.Selected);

        if (selectedCircles.length === 0 && canEdit) {
            const circleId = await dotNet.invokeMethodAsync("GetGuid");
            const newCircle = new Circle(currentMouseState.X - _diameter / 2, currentMouseState.Y - _diameter / 2, _diameter, "", circleId, false);
            newCircle.Selected = true;
            circles.push(newCircle);
            await dotNet.invokeMethodAsync("OnSelectedCircleChanged", JSON.stringify(newCircle));
        } else if (selectedCircles.length === 1) {
            await dotNet.invokeMethodAsync("OnSelectedCircleChanged", JSON.stringify(selectedCircles[0]));
        }
    }

    // // TODO: drag the selected circle
    // if (canEdit) {
    //     if (previousMouseState.LeftButton === "down" && currentMouseState.LeftButton === "down") {

    //         if (grabCountDownBuffer > 0) {
    //             grabCountDownBuffer -= deltaTime;
    //             return;
    //         }

    //         const grabbedCircles = circles.filter(x => x.Grabbed);

    //         if (grabbedCircles.length === 1) {
    //             const grabbedCircle = grabbedCircles[0];

    //             grabbedCircle.Position.Left = currentMouseState.X - grabbedCircle.Diameter / 2;
    //             grabbedCircle.Position.Top = currentMouseState.Y - grabbedCircle.Diameter / 2;
    //         } else if (grabbedCircles.length === 0) {
    //             circles.forEach(circle => {
    //                 circle.Grabbed = circle.IsCollidingWithMouse(currentMouseState);
    //             });
    //         }
    //     }
    // }

    // set the current hovered circle
    circles.forEach(circle => circle.Hovering = circle.IsCollidingWithMouse(currentMouseState));


    if (circles.filter(x => x.Hovering && x.EmployeeFullName).length > 0) {
        const spyingCircle = circles.find(x => x.Hovering && x.EmployeeFullName);
        const tooltip = document.getElementById("tooltip");

        tooltip.style.z_index = 1;
        tooltip.style.opacity = 1;
        tooltip.style.visibility = "visible";

        const x = spyingCircle.Position.Left + (spyingCircle.Diameter * 2);
        const y = spyingCircle.Position.Top;
        tooltip.style.left = `${x}px`;
        tooltip.style.top = `${y}px`;

        tooltip.innerHTML = spyingCircle.EmployeeFullName;

        if (!canEdit && spyingCircle.Taken && spyingContact !== spyingCircle.EmployeeFullName) {
            spyingContact = spyingCircle.EmployeeFullName;
            await dotNet.invokeMethodAsync("OnSpying",
                JSON.stringify({ Workspace: spyingCircle.Name, Victim: spyingContact }));
        }
    }

    if (circles.filter(x => x.Hovering).length === 0) {
        const tooltip = document.getElementById("tooltip");

        tooltip.style.z_index = -1;
        tooltip.style.opacity = 0;
        tooltip.style.visibility = "hidden";

        spyingContact = "";
    }

    const canvas = document.getElementById('floorplan-canvas');

    // change the mouse pointer if hovering a circle
    if(circles.filter(x => x.Grabbed).length > 0) {
        canvas.style.cursor = "grabbing";
    } else if (circles.filter(x => x.Hovering).length > 0) {
        canvas.style.cursor = "pointer";
    } else {
        canvas.style.cursor = "default";
    }

    // persists the current mouse state for the next frame
    previousMouseState = currentMouseState.clone();
}

function Draw(deltaTime) {
    const canvas = document.getElementById('floorplan-canvas');
    const context = canvas.getContext("2d");

    context.globalCompositeOperation = 'source-over';

    context.font = '10pt Calibri';

    // clear canvas
    context.clearRect(0, 0, canvas.width, canvas.height);

    context.globalAlpha = 0.5;
    context.drawImage(floorplan, 0, 0);
    context.globalAlpha = 1.0;

    // set the background of the circle
    circles
        .filter(x => !x.Taken)
        .forEach(circle => {
            const backgroundImage = circle.Hovering ? availableHoveredBackgroundImage : availableBackgroundImage;
            context.drawImage(backgroundImage, circle.Position.Left, circle.Position.Top, circle.Diameter, circle.Diameter);
        });

    circles
        .filter(x => x.Taken)
        .forEach(circle => {
            const backgroundImage = circle.Hovering ? selectedBackgroundImage : takenBackgroundImage;
            context.drawImage(backgroundImage, circle.Position.Left, circle.Position.Top, circle.Diameter, circle.Diameter);
        });

    circles
        .filter(x => x.Selected)
        .forEach(circle => {
            context.drawImage(selectedBackgroundImage, circle.Position.Left, circle.Position.Top, circle.Diameter, circle.Diameter);
        });

    // draw the icon inside of the circle
    circles
        .filter(x => x.Taken)
        .forEach(circle => {
            const iconImage = circle.IsFirstAidAttendant && circle.IsFloorEmergencyOfficer ? multipleRolesImage
                : circle.IsFirstAidAttendant ? firstAidAttendantImage
                : circle.IsFloorEmergencyOfficer ? floorEmergencyOfficerImage
                : userImage;

            const degrees = 315;
            const radius = circle.Diameter / 2;
            const offset = 2; // we want the icon to fit inside the circle so we set an offset

            // set the origin of the circle
            const origin = new Position(circle.Position.Left + radius, circle.Position.Top + radius);

            // calculate the coordinates of the top left corner of the circle
            const x = (-1 * (radius - offset) * Math.sin(degrees)) + origin.Left;
            const y = (-1 * (radius - offset) * Math.cos(degrees)) + origin.Top;

            // calculate the width of the largest square that fits inside the circle
            const width = (radius - offset) * Math.sqrt(2);

            context.drawImage(iconImage, x, y, width, width);
        });

    context.globalAlpha = 1.0;

    // using the width of the upper case later 'M' is a hack to get the font height, but it works...
    const fontHeight = context.measureText("M").width;

    // draw the name of the circle in the center of the circle
    circles
        .filter(x => !x.Taken)
        .forEach(circle => {
            const fontWidth = context.measureText(circle.Name).width;
            context.fillText(circle.Name, circle.Position.Left + circle.Diameter / 2 - fontWidth / 2, circle.Position.Top + fontHeight + circle.Diameter / 2 - fontHeight / 1.5);
        });

    if (canEdit) {
        context.fillText(`(${currentMouseState.X}, ${currentMouseState.Y})`, 3, canvas.height - 1);

        // TODO: remove, debug only
        context.fillText(`FPS: ${_frameCounter.AverageFramesPerSecond}`, 3, canvas.height - 20);
        context.fillText(`deltaTime: ${deltaTime}`, 3, canvas.height - 40);
    }
}

async function gameLoop(timeStamp) {
    if (!shouldRender) {
        return;
    }

    const canvas = document.getElementById('floorplan-canvas');

    if (canvas === null) {
        return;
    }

    // Calculate the number of seconds passed since the last frame
    const deltaTime = timeStamp - previousTimeStamp;
    previousTimeStamp = timeStamp;

    // Calculate fps
    _frameCounter.Update(deltaTime);

    await Update(deltaTime); // Perform the update operation
    Draw(deltaTime);   // Perform the drawing operation

    // The loop function has reached it's end. Keep requesting new frames
    window.requestAnimationFrame(gameLoop);
}

export let dotNet = null;
export let locale = null;

export function register(dotNetReference, loc, mode) {
    dotNet = dotNetReference;
    locale = loc;
    canEdit = mode === "editMode";
}

export async function start(imagedata, circlesJson) {
    resetState();

    const canvas = document.getElementById('floorplan-canvas');

    canvas.removeEventListener("mousemove", onMouseMove);
    canvas.addEventListener("mousemove", onMouseMove);

    canvas.removeEventListener("mousedown", onMouseDown);
    canvas.addEventListener("mousedown", onMouseDown);

    canvas.removeEventListener("mouseup", onMouseUp);
    canvas.addEventListener("mouseup", onMouseUp);

    canvas.removeEventListener("touchstart", onTouchStart);
    canvas.addEventListener("touchstart", onTouchStart);

    canvas.removeEventListener("touchend", onTouchEnd);
    canvas.addEventListener("touchend", onTouchEnd);

    // const holder = document.getElementById('canvasHolder');

    // if (holder) {
    //     canvas.width = holder.clientWidth;
    //     canvas.height = holder.clientHeight;

    //     canvas.style.width ='100%';
    //     canvas.style.height='100%';
    // }

    floorplan.src = imagedata;
    await floorplan.decode();

    // floorplan.onload = function() {
    //     // alert(this.width + 'x' + this.height);
    //     canvas.width = this.width;
    //     canvas.height = this.height;

    //     initCircles(circlesJson);

    //     shouldRender = true;
    //     window.requestAnimationFrame(gameLoop);
    // }

    availableBackgroundImage.src = '/img/floorplan/circle_available_icon.svg';
    availableHoveredBackgroundImage.src = '/img/floorplan/circle_available_hover_icon.svg';
    takenBackgroundImage.src = '/img/floorplan/circle_taken_icon.svg';
    selectedBackgroundImage.src = '/img/floorplan/circle_selected_icon.svg';
    firstAidAttendantImage.src = '/img/floorplan/first_aid_attendant_icon.svg';
    floorEmergencyOfficerImage.src = '/img/floorplan/floor_emergency_officer_icon.svg';
    multipleRolesImage.src = '/img/floorplan/multiple_roles_icon.svg';
    userImage.src = '/img/floorplan/user_icon.svg';

    canvas.width = floorplan.width;
    canvas.height = floorplan.height;

    initCircles(circlesJson);

    shouldRender = true;
    window.requestAnimationFrame(gameLoop);

    function initCircles(circlesJson) {
        const tempCircles = JSON.parse(circlesJson);

        tempCircles.forEach(circle => {
            const newCircle = new Circle(circle.Position.Left, circle.Position.Top, _diameter, circle.Name, circle.Id, circle.Selected);
            newCircle.Taken = circle.Taken;
            newCircle.EmployeeFullName = circle.EmployeeFullName;
            newCircle.IsFirstAidAttendant = circle.IsFirstAidAttendant;
            newCircle.IsFloorEmergencyOfficer = circle.IsFloorEmergencyOfficer;
            circles.push(newCircle);
        });
    }
}

export function updateCircle(data) {
    const circle = JSON.parse(data);

    circles
        .filter(x => x.Id === circle.Id)
        .forEach(x => {
            x.Position.Left = circle.Position.Left;
            x.Position.Top = circle.Position.Top;
            x.Name = circle.Name;
            x.EmployeeFullName = circle.EmployeeFullName;
        });
}

export function setSelectedCircle(circleId) {
    circles
        .forEach(x => {
            x.Selected = x.Id === circleId;
        });
}

// export function deleteSelectedCircle() {
//     const temp = circles.filter(x => !x.Selected);

//     while (circles.length > 0) {
//         circles.pop();
//     }

//     temp.forEach(circle => {
//         circles.push(circle);
//     });
// }

export function stop() {
    shouldRender = false;
}
