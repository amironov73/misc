import {Plugin} from 'obsidian';

export default class NeatPlugin extends Plugin {

  /*
      Структура досье:
      Наименование
      путь к файлу картинки|необязательный размер по ширине
      Свойство1: значение
      Свойство2: значение
   */

  async onload() {
    this.registerMarkdownCodeBlockProcessor ('dossier', (source, el) => {
      const rows = source.split ("\n").filter (row => row.length > 0)
      if (rows.length > 1) {
        const title = rows[0]
        let imageFile = rows[1]
        let width = 300

        if (imageFile.contains ('|')) {
          const parts = imageFile.split ('|')
          imageFile = parts[0]
          width = parseInt (parts[1])
        }

        const activeFile = this.app.workspace.getActiveFile()!
        if (activeFile) {
          let activePath = this.app.vault.getResourcePath (activeFile)
          activePath = activePath.substring (0, activePath.lastIndexOf ("/"))
          const fileName = activePath + '/' + imageFile
          const div = el.createDiv ({
            attr: {'style': 'clear: both;'}
          })

          // картинка
          div.createEl ("img", {
            attr:
              {
                'width': width,
                'style': 'float: left; margin: 4mm;',
                'src': fileName
              }
          })

          // подпись к картинке
          div.createDiv ({
            attr: {'style': 'font-size: xx-large; font-weight: bold;'}
          })
            .innerText = title

          const table = div.createEl ("table", {
            attr: {
              'border': 0,
              'style': 'font-size: x-large;'
            }
          })

          // табличка с характеристиками
          for (let i = 2; i < rows.length; i++) {
            const parts = rows[i].split (':', 2)
            if (parts.length > 1) {
              const tr = table.createEl ("tr", {
                attr: {'style': 'padding: 2mm;'}
              })
              tr.createEl ("td", {
                attr: {'style': 'vertical-align: top; font-weight: bold;'}
              }).innerText = parts[0].trim()
              tr.createEl ("td", {
                attr: {'style': 'vertical-align: top;'}
              }).innerText = parts[1].trim()
            }
          }
        }

        // очистка
        el.createDiv ({
          attr: {'style': 'clear: both;'}
        })
      }
    })
  }

  onunload() {
    // пустое тело метода
  }

}

