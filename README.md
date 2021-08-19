
[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![License][license-shield]][license-url]

<!-- PROJECT LOGO -->
<br />
<p align="center">
  <a href="https://github.com/VisualMQ/VisualMQ/">
    <img src="visualmq-logo.png" alt="Logo" width="300">
  </a>

  <h3 align="center">VisualMQ</h3>

  <p align="center">
    VisualMQ is a 3D visualisation tool for the IBM MQ Platform.
    <br />
    <a href="https://github.com/othneildrew/Best-README-Template"><strong>Video of VisualMQ</strong></a>
    <br />
    <br />
    <a href="https://visualmq.diallom.com/demo/">Demo Version</a>
    ·
    <a href="https://github.com/VisualMQ/VisualMQ/issues">Bug Report</a>
    ·
  
  </p>
</p>



<!-- TABLE OF CONTENTS -->
<details open="open">
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About</a>
      <ul>
        <li><a href="#built-with">Tools used</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#Usage-Prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li><a href="#usage">Usage</a></li>
    <li><a href="#contributing">Contributing</a></li>
    <li><a href="#VisualMQ-Team">Contact</a></li>
    <li><a href="#acknowledgements">Acknowledgements</a></li>
     <li><a href="#license">License</a></li>
  </ol>
</details>



<!-- ABOUT THE PROJECT -->
## About The Project

[![Product Name Screen Shot][product-screenshot]](https://visualmq.diallom.com/demo)

VisualMQ is a supportive application for IBM MQ systems. Created in Unity, VisualMQ is designed to create 3D representations of MQ systems. For this purpose, we have created novel 3D models representing the different components of MQ. VisualMQ is aimed at being a novel tool that revolutionizes the way end-users obtain an understanding of the systems functioning.  As opposed to the current inspection methods for MQ systems,  the relationship between different components is emphasized and data is represented in such a way to make it as easy as possible to understand. 


### Built With
* [IBM MQ](https://www.ibm.com/docs/en/ibm-mq)
* [Unity](https://unity.com/)
* [Blender](https://www.blender.org/)



<!-- GETTING STARTED -->
## Getting Started


### Usage Prerequisites

Usage of VisualMQ requires a valid set of queue manager details. For this valid IBM Cloud details are required. For this we recommend observing the [official tutorial](https://www.ibm.com/cloud/garage/dte/tutorial/tutorial-mq-ibm-cloud) the first two steps "Creating MQ Services On IBM Cloud" and "Creating A Queue Manager On IBM Cloud" to create one or more queue managers. For each of these queue managers the following credentials will be necessary:
- API KEY
- Username
- Host URL
- Queue Manager Name

The tips under the tutorial's section "Administering MQ Using MQ Console" give partial instruction on how to retrieve these. 


### Installation

VisualMQ is available in a multitude of ways for usage purposes or development.

#### Binaries

MacOS and Windows binaries are available under [releases](https://github.com/VisualMQ/VisualMQ/releases).

#### Web demo (TBD)
VisualMQ is available for quick evaluation as a [web demo](https://visualmq.diallom.com/demo/).


#### Development in Unity
1. Ensure Unity version `2020.3.11f1` is installed.
2. Clone this repository
3. Open the root of this directory within Unity

- For development: see the section on contributing.
- For building / running:
  - Building: https://docs.unity3d.com/Manual/BuildSettings.html
  - Running: https://docs.unity3d.com/Manual/GameView.html





<!-- USAGE EXAMPLES -->
## Usage

TBA Screenshots and video





<!-- CONTRIBUTING -->
## Contributing

Contributing is done by following the "Getting Started" instructions for development. With exceptions of the models made in Blender, VisualMQ is entirely made within Unity. 

[Microsoft Guide](https://docs.microsoft.com/en-us/archive/msdn-magazine/2014/august/unity-developing-your-first-game-with-unity-and-csharp)  

[Unity Manual](https://docs.unity3d.com/Manual/index.html) 

Application layout:
- C\# Application logic pertaining to gameobjects is found under `Assets/Scripts/`.
  - `Details` Contains files implementing logic for the information panels upon clicking on the correspondingly named entity.
  - `MQ` Contains files implementing the logic for interacting with the rest-api and retrieving/parsing data.
  - `Navigation` Contains files implementing the UI / Navigational control logic.
  - `Rendering` Contains the core logic for visualisation. (TBD: This could be more elaborate but I would rather wait till we have a diagram or something final)
- Application is contained into a single `MainScene`




<!-- CONTACT -->
## VisualMQ Team

- [Madiou Diallo](https://github.com/Diallo) - [LinkedIn](https://linkedin.com/in/mdiallos)
- [Lukas Cerny](https://github.com/lukasotocerny)
- [Yue Xu](https://github.com/yuexu-98)  
- [Shucheng Tian](https://github.com/phillip-tian) - [LinkedIn](https://www.linkedin.com/in/shucheng-tian/)
- [Yifan Hu](https://github.com/huyifanx) - [LinkedIn](https://www.linkedin.com/in/yifanhuleo/)
- [Xu Zhang](https://github.com/Orange0719)

VisualMQ project link: [https://github.com/VisualMQ/](https://github.com/VisualMQ/)



<!-- ACKNOWLEDGEMENTS -->
## Acknowledgements

We would like to express our gratitude towards the IBM supervisors: [John McNamara](https://github.com/IBMIXN), [Adrian Osadcenco](#) and [Richard Coppen](https://github.com/rcoppen). The time, energy and support they have dedicated to this project have had a greatly positive impact on the end-result. Furthermore, we would also like to thank the MQ outreach team. The [MQ workshop](https://developer.ibm.com/series/badge-ibm-mq-developer-essentials/) they provided us with was extremely informative and provided us with a significant amount of specific MQ knowledge.

\- VisualMQ Team


<!-- LICENSE -->
## License

Distributed under the TBA License. See `LICENSE` for more information.




<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[contributors-shield]: https://img.shields.io/github/contributors/VisualMQ/visualmq.svg?style=for-the-badge
[contributors-url]: https://github.com/VisualMQ/VisualMQ/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/VisualMQ/visualmq.svg?style=for-the-badge
[forks-url]: https://github.com/VisualMQ/VisualMQ/network/members
[stars-shield]: https://img.shields.io/github/stars/VisualMQ/visualmq.svg?style=for-the-badge
[stars-url]: https://github.com/VisualMQ/VisualMQ/stargazers
[issues-shield]: https://img.shields.io/github/issues/VisualMQ/visualmq.svg?style=for-the-badge
[issues-url]: https://github.com/VisualMQ/VisualMQ/issues
[license-shield]: https://img.shields.io/github/license/VisualMQ/visualmq.svg?style=for-the-badge
[license-url]: https://github.com/VisualMQ/VisualMQ/blob/master/LICENSE.txt
[product-screenshot]: visualmq-screenshot.png
